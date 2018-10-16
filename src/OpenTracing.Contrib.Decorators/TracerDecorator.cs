using System;
using System.Collections.Generic;
using System.Text;
using OpenTracing.Propagation;

namespace OpenTracing.Contrib.Decorators
{
    class TracerDecorator : ITracer
    {
        private readonly ITracer _tracer;
        private readonly BuildersDecoratorHooks _hooks;

        public TracerDecorator(ITracer tracer, BuildersDecoratorHooks hooks)
        {
            _tracer = tracer;
            _hooks = hooks;
            ScopeManager = new ScopeManagerDecorator(tracer.ScopeManager, this, _hooks);
        }

        public virtual IScopeManager ScopeManager { get; }

        public virtual ISpan ActiveSpan => ScopeManager.Active?.Span;

        public virtual ISpanBuilder BuildSpan(string operationName)
        {
            var spanBuilder = _tracer.BuildSpan(operationName);
            return new SpanBuilderDecorator(spanBuilder, this, operationName, _hooks);
        }

        public virtual ISpanContext Extract<TCarrier>(IFormat<TCarrier> format, TCarrier carrier) => _tracer.Extract(format, carrier);

        public virtual void Inject<TCarrier>(ISpanContext spanContext, IFormat<TCarrier> format, TCarrier carrier) => _tracer.Inject(spanContext, format, carrier);
    }
}
