using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTracing.Contrib.Decorators
{
    public delegate OnSpanFinished OnSpanStartedWithFinishCallback(ISpan span, string operationName);
    public delegate void OnSpanStarted(ISpan span, string operationName);
    public delegate void OnSpanActivated(ISpan span, string operationName);
    public delegate void OnSpanFinished(ISpan span, string operationName);

    public struct LogKeyValue { public string key; public object value; }
    public delegate void OnSpanLog(ISpan span, string operationName, DateTimeOffset timestamp, LogKeyValue[] fields);


    class BuildersDecoratorHooks
    {
        public OnSpanStartedWithFinishCallback OnSpanStartedWithFinishCallback { get; set; } = (span, operationName) => ((sp, str) => { });

        public OnSpanStarted OnSpanStarted { get; set; } = (span, operationName) => { };
        public OnSpanActivated OnSpanActivated { get; set; } = (span, operationName) => { };
        public OnSpanFinished OnSpanFinished { get; set; } = (span, operationName) => { };

        public OnSpanLog OnSpanLog { get; set; } = (span, operationName, timestamp, fields) => { };
    }

    class SpanDecoratorHooks
    {
        public SpanDecoratorHooks(BuildersDecoratorHooks mainHooks, OnSpanFinished callBack)
        {
            OnSpanStarted = mainHooks.OnSpanStarted;
            OnSpanActivated = mainHooks.OnSpanActivated;
            OnSpanFinished = (OnSpanFinished)OnSpanFinished.Combine(mainHooks.OnSpanFinished, callBack);
            OnSpanLog = mainHooks.OnSpanLog;
        }

        public OnSpanStarted OnSpanStarted { get; }
        public OnSpanActivated OnSpanActivated { get; }
        public OnSpanFinished OnSpanFinished { get; }

        public OnSpanLog OnSpanLog { get; }
    }
}
