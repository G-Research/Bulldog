using System;
using System.Threading;
using CommandLine;
using CommandLine.Text;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog.Extensions.Logging;

namespace Bulldog
{
    public abstract class ExitCodeBase
    {
        public static int Success => 0;
        public static int InvalidArguments => -1;
        public static int UnhandledException => -9;
    }

    public abstract class ToolBase<T> where T : IToolOptions
    {
        protected readonly CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();
        protected Microsoft.Extensions.Logging.ILogger Logger;
        private LoggingLevelSwitch _loggingLevelSwitch;
        protected string[] _inputArguments;

        protected bool _handleCancellation = true;

        protected virtual bool MonitorForTaskKill => false;

        protected virtual string SerilogOutputTemplate => "[{Timestamp:HH:mm:ss.fff}][{Level:u3}] [{ThreadID}] {Message:lj}{NewLine}{Exception}";

        private void CurrentDomainOnProcessExit(object sender, EventArgs eventArgs)
        {
            try
            {
                Log.Error("Process shutdown detected. Cancelling any remaining tasks.");
            }
            finally
            {
                Cancel();
            }
        }

        private void Cancel()
        {
            CancellationTokenSource.Cancel();
            Log.Error("Cancellation completed.");
        }

        private void OnHiddenWindowClose(uint message)
        {
            Log.Error("Received Windows CLOSE message. Cancelling remaining tasks.");
            Cancel();
        }

        private void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            try
            {
                Log.Error("Cancellation requested. Cancelling remaining tasks.");
            }
            finally
            {
                Cancel();
                e.Cancel = _handleCancellation;// It is possible to override this and get the process terminated.
            }
        }

        protected void SetMinimumLogLevel(LogEventLevel logEventLevel)
        {
            if (_loggingLevelSwitch.MinimumLevel != logEventLevel)
            {
                Log.Information($"Minimum log level has been set to {logEventLevel}.");
                _loggingLevelSwitch.MinimumLevel = logEventLevel;
            }
        }

        protected abstract Task<int> Run(T options);

        protected virtual bool TryGetOptions(string[] args, out T options)
        {
            Parser parser = new Parser((ps) => ps.HelpWriter = null);

            var parserResult = parser.ParseArguments<T>(args);

            options = default(T);
            if (parserResult.Tag == ParserResultType.NotParsed)
            {
                Log.Error($"Failed to parse command line:{Environment.NewLine} {HelpText.AutoBuild(parserResult)}");
                return false;
            }

            options = ((Parsed<T>)parserResult).Value;

            return true;
        }

        protected ServiceProvider ServiceProvider { get; private set; }

        public async Task<int> Run(string[] args)
        {
            ConfigureLogging();

            if (!TryGetOptions(args, out T options))
            {
                return ExitCodeBase.InvalidArguments;
            }

            SetMinimumLogLevel(options.LogLevel);

            // Generate a provider
            ServiceProvider = ConfigureServices(options);

            Logger = ServiceProvider.GetRequiredService<ILogger<ToolBase<T>>>();

            _inputArguments = args;

            try
            {
                Console.CancelKeyPress += OnCancelKeyPress;

                if (MonitorForTaskKill && Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    HiddenWindow.HiddenWindow.OnClose += OnHiddenWindowClose;
                    HiddenWindow.HiddenWindow.Create();
                }

                AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnProcessExit;

                return await Run(options);
            }
            catch (Exception exception)
            {
                Log.Error(exception, "Unhandled exception in Tool execution:");
                return ExitCodeBase.UnhandledException;
            }
            finally
            {
                Log.CloseAndFlush();
                Console.CancelKeyPress -= OnCancelKeyPress;

                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    HiddenWindow.HiddenWindow.OnClose -= OnHiddenWindowClose;
                }

                AppDomain.CurrentDomain.ProcessExit -= CurrentDomainOnProcessExit;
            }
        }

        protected virtual void ConfigureServices(IServiceCollection serviceCollection, T options)
        {

        }

        private ServiceProvider ConfigureServices(T options)
        {
            LoggerProviderCollection loggerProviders = new LoggerProviderCollection();
            IServiceCollection services = new ServiceCollection();

            services.AddSingleton(loggerProviders);

            services.AddSingleton<ILoggerFactory>(sc =>
            {
                var providerCollection = sc.GetService<LoggerProviderCollection>();
                var factory = new SerilogLoggerFactory(null, true, providerCollection);

                foreach (var provider in sc.GetServices<ILoggerProvider>())
                    factory.AddProvider(provider);

                return factory;
            });

            services.AddLogging(l => l.AddSerilog());

            services.AddTransient(sp=> sp.GetService<ILoggerFactory>().CreateLogger("Default"));

            ConfigureServices(services, options);

            var serviceProvider = services.BuildServiceProvider(new ServiceProviderOptions() { ValidateOnBuild = true, ValidateScopes = true });
            return serviceProvider;
        }

        private void ConfigureLogging()
        {
            _loggingLevelSwitch = new LoggingLevelSwitch();
            Log.Logger = new LoggerConfiguration().MinimumLevel.ControlledBy(_loggingLevelSwitch)
                .Enrich.With(new ThreadIdEnricher())
                .WriteTo.Console(standardErrorFromLevel: LogEventLevel.Error, outputTemplate: SerilogOutputTemplate)
                .CreateLogger();
        }
    }
}
