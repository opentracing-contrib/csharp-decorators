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

    class DecoratorHooks
    {
        public OnSpanStarted OnSpanStarted { get; set; } = (span, operationName) => { };
        public OnSpanActivated OnSpanActivated { get; set; } = (span, operationName) => { };
        public OnSpanFinished OnSpanFinished { get; set; } = (span, operationName) => { };
    }
}
