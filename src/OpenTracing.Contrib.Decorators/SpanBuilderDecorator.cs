using System;
using System.Collections.Generic;
using System.Text;
using OpenTracing.Tag;

namespace OpenTracing.Contrib.Decorators
{
    class SpanBuilderDecorator : ISpanBuilder
    {
        private readonly ISpanBuilder _spanBuilder;
        private readonly ITracer _tracer;
        private readonly string _operationName;
        private readonly BuildersDecoratorHooks _hooks;

        public SpanBuilderDecorator(ISpanBuilder spanBuilder, ITracer tracer, string operationName, BuildersDecoratorHooks hooks)
        {
            _spanBuilder = spanBuilder;
            _tracer = tracer;
            _operationName = operationName;
            _hooks = hooks;
        }

        public virtual ISpanBuilder AddReference(string referenceType, ISpanContext referencedContext) { _spanBuilder.AddReference(referenceType, referencedContext); return this; }

        public virtual ISpanBuilder AsChildOf(ISpanContext parent) { _spanBuilder.AsChildOf(parent); return this; }

        public virtual ISpanBuilder AsChildOf(ISpan parent) { _spanBuilder.AsChildOf(parent); return this; }

        public virtual ISpanBuilder IgnoreActiveSpan() { _spanBuilder.IgnoreActiveSpan(); return this; }


        public virtual ISpan Start()
        {
            var span = _spanBuilder.Start();

            _hooks.OnSpanStarted(span, _operationName);
            var callback = _hooks.OnSpanStartedWithFinishCallback(span, _operationName);
            var spanHooks = new SpanDecoratorHooks(_hooks, callback);

            var decorator = new SpanDecorator(span, _tracer, _operationName, spanHooks);
            ApplyPostponedTags(decorator);
            return decorator;
        }

        public virtual IScope StartActive() => StartActive(true);

        public virtual IScope StartActive(bool finishSpanOnDispose)
        {
            ISpan span = Start();
            return _tracer.ScopeManager.Activate(span, finishSpanOnDispose);
        }

        public virtual ISpanBuilder WithStartTimestamp(DateTimeOffset timestamp) { _spanBuilder.WithStartTimestamp(timestamp); return this; }


        // In the next methods, I need to postpone the tag application, in order to able to call hooks,
        // otherwise the span is created with no chance to catch the tags.

        List<Action<ISpan>> _tagsApplies = new List<Action<ISpan>>();

        private void ApplyPostponedTags(ISpan span)
        {
            foreach (var action in _tagsApplies)
                action(span);
        }

        public virtual ISpanBuilder WithTag(string key, string value) { _tagsApplies.Add(s => s.SetTag(key, value)); return this; }

        public virtual ISpanBuilder WithTag(string key, bool value) { _tagsApplies.Add(s => s.SetTag(key, value)); return this; }

        public virtual ISpanBuilder WithTag(string key, int value) { _tagsApplies.Add(s => s.SetTag(key, value)); return this; }

        public virtual ISpanBuilder WithTag(string key, double value) { _tagsApplies.Add(s => s.SetTag(key, value)); return this; }

        public virtual ISpanBuilder WithTag(BooleanTag tag, bool value) { _tagsApplies.Add(s => s.SetTag(tag, value)); return this; }

        public virtual ISpanBuilder WithTag(IntOrStringTag tag, string value) { _tagsApplies.Add(s => s.SetTag(tag, value)); return this; }

        public virtual ISpanBuilder WithTag(IntTag tag, int value) { _tagsApplies.Add(s => s.SetTag(tag, value)); return this; }

        public virtual ISpanBuilder WithTag(StringTag tag, string value) { _tagsApplies.Add(s => s.SetTag(tag, value)); return this; }
    }
}
