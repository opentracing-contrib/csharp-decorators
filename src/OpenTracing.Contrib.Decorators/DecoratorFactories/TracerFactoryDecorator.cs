using System;
using System.Collections.Generic;
using System.Text;
using OpenTracing.Propagation;

namespace OpenTracing.Contrib.Decorators.DecoratorFactories
{
    sealed class TracerFactoryDecorator : ITracer
    {
        private readonly ITracer _tracer;
        private readonly InternalScopeManagerDecoratorFactory _scopeManagerDecoratorFactory;
        private readonly InternalSpanDecoratorFactory _spanDecoratorFactory;
        private readonly InternalSpanBuilderDecoratorFactory _spanBuilderDecoratorFactory;
        private readonly InternalSpanContextDecoratorFactory _spanContextDecoratorFactory;

        public TracerFactoryDecorator(
            ITracer tracer,
            InternalScopeManagerDecoratorFactory scopeManagerDecoratorFactory,
            InternalSpanDecoratorFactory spanDecoratorFactory,
            InternalSpanBuilderDecoratorFactory spanBuilderDecoratorFactory,
            InternalSpanContextDecoratorFactory spanContextDecoratorFactory
            )
        {
            _tracer = tracer;
            _scopeManagerDecoratorFactory = scopeManagerDecoratorFactory ?? throw new ArgumentNullException(nameof(scopeManagerDecoratorFactory));
            _spanDecoratorFactory = spanDecoratorFactory ?? throw new ArgumentNullException(nameof(spanDecoratorFactory));
            _spanBuilderDecoratorFactory = spanBuilderDecoratorFactory ?? throw new ArgumentNullException(nameof(spanBuilderDecoratorFactory));
            _spanContextDecoratorFactory = spanContextDecoratorFactory ?? throw new ArgumentNullException(nameof(spanContextDecoratorFactory));
        }

        public IScopeManager ScopeManager => _scopeManagerDecoratorFactory(_tracer.ScopeManager, Defaults.UNKNOWN_OPERATION_NAME);

        public ISpan ActiveSpan => _spanDecoratorFactory(_tracer.ActiveSpan, Defaults.UNKNOWN_OPERATION_NAME);

        public ISpanBuilder BuildSpan(string operationName) => _spanBuilderDecoratorFactory(_tracer.BuildSpan(operationName), operationName);

        public ISpanContext Extract<TCarrier>(IFormat<TCarrier> format, TCarrier carrier) => _spanContextDecoratorFactory(_tracer.Extract(format, carrier));

        public void Inject<TCarrier>(ISpanContext spanContext, IFormat<TCarrier> format, TCarrier carrier) => _tracer.Inject(spanContext, format, carrier);
    }
}
