using System;
using System.Collections.Generic;
using System.Text;
using OpenTracing.Tag;

namespace OpenTracing.Contrib.Decorators.DecoratorFactories
{
    sealed class SpanBuilderFactoryDecorator : ISpanBuilder
    {
        private readonly ISpanBuilder _spanBuilder;
        private readonly InternalSpanDecoratorFactory _spanDecoratorFactory;
        private readonly InternalScopeDecoratorFactory _scopeDecoratorFactory;
        private readonly string _operationName;

        public SpanBuilderFactoryDecorator(ISpanBuilder spanBuilder, InternalSpanDecoratorFactory spanDecoratorFactory, InternalScopeDecoratorFactory scopeDecoratorFactory, string operationName)
        {
            _spanBuilder = spanBuilder;
            _spanDecoratorFactory = spanDecoratorFactory ?? throw new ArgumentNullException(nameof(spanDecoratorFactory));
            _scopeDecoratorFactory = scopeDecoratorFactory ?? throw new ArgumentNullException(nameof(scopeDecoratorFactory));
            _operationName = operationName;
        }

        public ISpanBuilder AddReference(string referenceType, ISpanContext referencedContext) { _spanBuilder.AddReference(referenceType, referencedContext); return this; }

        public ISpanBuilder AsChildOf(ISpanContext parent) { _spanBuilder.AsChildOf(parent); return this; }

        public ISpanBuilder AsChildOf(ISpan parent) { _spanBuilder.AsChildOf(parent); return this; }

        public ISpanBuilder IgnoreActiveSpan() { _spanBuilder.IgnoreActiveSpan(); return this; }

        public ISpan Start() => _spanDecoratorFactory(_spanBuilder.Start(), _operationName);

        public IScope StartActive() => _scopeDecoratorFactory(_spanBuilder.StartActive(), _operationName);

        public IScope StartActive(bool finishSpanOnDispose) => _scopeDecoratorFactory(_spanBuilder.StartActive(finishSpanOnDispose), _operationName);

        public ISpanBuilder WithStartTimestamp(DateTimeOffset timestamp) { _spanBuilder.WithStartTimestamp(timestamp); return this; }

        public ISpanBuilder WithTag(string key, string value) { _spanBuilder.WithTag(key, value); return this; }

        public ISpanBuilder WithTag(string key, bool value) { _spanBuilder.WithTag(key, value); return this; }

        public ISpanBuilder WithTag(string key, int value) { _spanBuilder.WithTag(key, value); return this; }

        public ISpanBuilder WithTag(string key, double value) { _spanBuilder.WithTag(key, value); return this; }

        public ISpanBuilder WithTag(BooleanTag tag, bool value) { _spanBuilder.WithTag(tag, value); return this; }

        public ISpanBuilder WithTag(IntOrStringTag tag, string value) { _spanBuilder.WithTag(tag, value); return this; }

        public ISpanBuilder WithTag(IntTag tag, int value) { _spanBuilder.WithTag(tag, value); return this; }

        public ISpanBuilder WithTag(StringTag tag, string value) { _spanBuilder.WithTag(tag, value); return this; }
    }
}
