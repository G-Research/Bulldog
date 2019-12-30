using Bulldog;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace TestTool
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var testTool = new TestTool();
            return await testTool.Run(args);
        }
    }

    public class DependencyClass
    {
        private ILogger _logger;

        /// <summary>
        /// Use the default ILogger so we can prove that ILogger registration works.
        /// </summary>
        public DependencyClass(ILogger logger)
        {
            _logger = logger;
        }

        public void Run()
        {
            _logger.LogInformation("Running something in my dependency class.");
        }
    }

    public class DummyClass
    {
        private readonly ILogger _logger;
        private readonly DependencyClass _dependencyClass;

        public DummyClass(ILogger<DummyClass> logger, DependencyClass dependencyClass)
        {
            _logger = logger;
            _dependencyClass = dependencyClass;
        }

        public void DoSomething()
        {
            _logger.LogInformation("I'm doing something.");

            _dependencyClass.Run();
        }
    }

    public class Options : OptionsBase
    {
        [Option()]
        public string Flag { get; set; }
    }

    public class TestTool : ToolBase<Options>
    {
        protected override bool MonitorForTaskKill => true;

        protected override void ConfigureServices(IServiceCollection serviceCollection, Options options)
        {
            serviceCollection.AddTransient<DummyClass>();
            serviceCollection.AddTransient<DependencyClass>();
        }

        protected override Task<int> Run(Options options)
        {
            Logger.LogInformation("Running the tool.");

            var dummyClass = ServiceProvider.GetService<DummyClass>();

            dummyClass.DoSomething();

            return Task.FromResult(0);
        }
    }
}
