using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTracing.Contrib.Decorators
{
    public delegate void OnSpanActivated(ISpan span, string operationName);
    public delegate void OnSpanFinished(ISpan span, string operationName);
    public delegate void OnSpanStarted(ISpan span, string operationName);

    public struct LogKeyValue { public string key; public object value; }
    public delegate void OnSpanLog(ISpan span, string operationName, DateTimeOffset timestamp, LogKeyValue[] fields);


    class DecoratorHooks
    {
        public OnSpanStarted OnSpanStarted { get; set; } = (span, operationName) => { };
        public OnSpanActivated OnSpanActivated { get; set; } = (span, operationName) => { };
        public OnSpanFinished OnSpanFinished { get; set; } = (span, operationName) => { };

        public OnSpanLog OnSpanLog { get; set; } = (span, operationName, timestamp, fields) => { };
    }
}
