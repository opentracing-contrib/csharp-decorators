using System;
using System.Collections.Generic;
using System.Text;

namespace OpenTracing.Contrib.Decorators
{
    class ScopeManagerDecorator : IScopeManager
    {
        private readonly ITracer _tracer;
        private readonly DecoratorHooks _hooks;
        private readonly IScopeManager _scopeManager;

        public ScopeManagerDecorator(IScopeManager scopeManager, ITracer tracer, DecoratorHooks hooks)
        {
            _scopeManager = scopeManager;
            _tracer = tracer;
            _hooks = hooks;
        }

        public virtual IScope Active => _scopeManager.Active;

        public IScope Activate(ISpan span, bool finishSpanOnDispose)
        {
            // Now we activate our span and not the wrapped one
            // We don't have to pass it the wrapped span as IScopeManager is a completely separate
            // concept that just happens to sit on ITracer (for easier accessibility).
            IScope scope = _scopeManager.Activate(span, finishSpanOnDispose);

            if (span is SpanDecorator decoratedSpan)
            {
                _hooks.OnSpanActivated(span, decoratedSpan._operationName);
            }

            return scope;
        }
    }
}
