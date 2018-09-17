using System;
using System.Collections.Generic;
using System.Text;
using OpenTracing.Contrib.Decorators.DecoratorFactories;

namespace OpenTracing.Contrib.Decorators
{
    public class TracerDecoratorBuilder
    {
        private readonly ITracer _tracer;
        public TracerDecoratorBuilder(ITracer tracer)
        {
            _tracer = tracer;
        }

        private TracerDecoratorFactory _tracerDecoratorFactory;
        public TracerDecoratorBuilder WithTracerDecorator(TracerDecoratorFactory factory) { _tracerDecoratorFactory = factory; return this; }

        private ScopeManagerDecoratorFactory _scopeManagerDecoratorFactory;
        public TracerDecoratorBuilder WithScopeManagerDecorator(ScopeManagerDecoratorFactory factory) { _scopeManagerDecoratorFactory = factory; return this; }

        private SpanDecoratorFactory _spanDecoratorFactory;
        public TracerDecoratorBuilder WithSpanDecorator(SpanDecoratorFactory factory) { _spanDecoratorFactory = factory; return this; }

        private SpanBuilderDecoratorFactory _spanBuilderDecoratorFactory;
        public TracerDecoratorBuilder WithSpanBuilderDecorator(SpanBuilderDecoratorFactory factory) { _spanBuilderDecoratorFactory = factory; return this; }

        private SpanContextDecoratorFactory _spanContextDecoratorFactory;
        public TracerDecoratorBuilder WithSpanContextDecorator(SpanContextDecoratorFactory factory) { _spanContextDecoratorFactory = factory; return this; }

        private ScopeDecoratorFactory _scopeDecoratorFactory;
        public TracerDecoratorBuilder WithScopeDecorator(ScopeDecoratorFactory factory) { _scopeDecoratorFactory = factory; return this; }

        public ITracer Build()
        {
            var tracerFactory = _tracerDecoratorFactory ?? DefaultDecoratorFactories.DefaultTracerDecoratorFactory;
            var scopeManagerFactory = _scopeManagerDecoratorFactory ?? DefaultDecoratorFactories.DefaultScopeManagerDecoratorFactory;
            var spanFactory = _spanDecoratorFactory ?? DefaultDecoratorFactories.DefaultSpanDecoratorFactory;
            var spanBuilderFactory = _spanBuilderDecoratorFactory ?? DefaultDecoratorFactories.DefaultSpanBuilderDecoratorFactory;
            var spanContextFactory = _spanContextDecoratorFactory ?? DefaultDecoratorFactories.DefaultSpanContextDecoratorFactory;
            var scopeFactory = _scopeDecoratorFactory ?? DefaultDecoratorFactories.DefaultScopeDecoratorFactory;

            InternalSpanContextDecoratorFactory spanContextDecoratorFactory = spanContext => new SpanContextFactoryDecorator(spanContextFactory(spanContext));
            InternalSpanDecoratorFactory spanDecoratorFactory = (span, operationName) => new SpanFactoryDecorator(spanFactory(span), spanContextDecoratorFactory);
            InternalScopeDecoratorFactory scopeDecoratorFactory = (scope, operationName, finishSpanOnDispose) => new ScopeFactoryDecorator(scopeFactory(scope), spanDecoratorFactory, operationName, finishSpanOnDispose);
            InternalScopeManagerDecoratorFactory scopeManagerDecoratorFactory = (scopeManager, operationName) => new ScopeManagerFactoryDecorator(scopeManagerFactory(scopeManager), scopeDecoratorFactory, operationName);
            InternalSpanBuilderDecoratorFactory spanBuilderDecoratorFactory = (spanBuilder, operationName) => new SpanBuilderFactoryDecorator(spanBuilderFactory(spanBuilder), spanDecoratorFactory, scopeDecoratorFactory, operationName);

            return new TracerFactoryDecorator(
                tracerFactory(_tracer),
                scopeManagerDecoratorFactory,
                spanDecoratorFactory,
                spanBuilderDecoratorFactory,
                spanContextDecoratorFactory
                );
        }
    }
}
