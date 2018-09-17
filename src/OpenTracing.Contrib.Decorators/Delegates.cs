using System;
using System.Collections.Generic;
using System.Text;

namespace OpenTracing.Contrib.Decorators
{
    public delegate ITracer TracerDecoratorFactory(ITracer tracer);
    public delegate IScopeManager ScopeManagerDecoratorFactory(IScopeManager scopeManager);
    public delegate ISpan SpanDecoratorFactory(ISpan span);
    public delegate ISpanBuilder SpanBuilderDecoratorFactory(ISpanBuilder spanBuilder);
    public delegate ISpanContext SpanContextDecoratorFactory(ISpanContext spanContext);
    public delegate IScope ScopeDecoratorFactory(IScope scope);

    public static class DefaultDecoratorFactories
    {
        public static ITracer DefaultTracerDecoratorFactory(ITracer tracer) => tracer;
        public static IScopeManager DefaultScopeManagerDecoratorFactory(IScopeManager scopeManager) => scopeManager;
        public static ISpan DefaultSpanDecoratorFactory(ISpan span) => span;
        public static ISpanBuilder DefaultSpanBuilderDecoratorFactory(ISpanBuilder spanBuilder) => spanBuilder;
        public static ISpanContext DefaultSpanContextDecoratorFactory(ISpanContext spanContext) => spanContext;
        public static IScope DefaultScopeDecoratorFactory(IScope scope) => scope;
    }

    public delegate void OnSpanActivated(ISpan span, string operationName);
    public delegate void OnSpanFinished(ISpan span, string operationName);

    public static class DefaultDecoratorHooks
    {
        public static void OnSpanActivated(ISpan span, string operationName) { }
        public static void OnSpanFinished(ISpan span, string operationName) { }
    }

}
