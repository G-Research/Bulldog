using System;
using CommandLine;
using Serilog.Events;

namespace Bulldog
{
    public abstract class OptionsBase : IToolOptions
    {
        [Option('v', "verbosity", Required = false, HelpText = "Minimum Logging level (Verbose,Debug,Information)", Default = LogEventLevel.Information)]
        public LogEventLevel LogLevel { get; set; }
    }

    public interface IToolOptions
    {
        LogEventLevel LogLevel { get; }
    }
}
