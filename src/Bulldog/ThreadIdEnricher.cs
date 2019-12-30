using Serilog.Core;
using Serilog.Events;
using System.Threading;

namespace Bulldog
{
    public class ThreadIdEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ThreadID", Thread.CurrentThread.ManagedThreadId.ToString()));
        }
    }
}