using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTracing.Tag;

namespace OpenTracing.Contrib.Decorators
{
    class SpanDecorator : ISpan
    {
        private readonly ISpan _span;
        private readonly ITracer _tracer;
        internal string _operationName;
        private readonly DecoratorHooks _hooks;

        public SpanDecorator(ISpan span, ITracer tracer, string operationName, DecoratorHooks hooks)
        {
            _span = span;
            _tracer = tracer;
            _operationName = operationName;
            _hooks = hooks;
        }

        public virtual ISpanContext Context => _span.Context;

        public virtual void Finish()
        {
            _span.Finish();
            _hooks.OnSpanFinished(_span, _operationName);
        }

        public virtual void Finish(DateTimeOffset finishTimestamp)
        {
            _span.Finish(finishTimestamp);
            _hooks.OnSpanFinished(_span, _operationName);
        }
        public virtual string GetBaggageItem(string key) => _span.GetBaggageItem(key);

        public virtual ISpan Log(IEnumerable<KeyValuePair<string, object>> fields)
        {
            _span.Log(fields);
            _hooks.OnSpanLog(_span, _operationName, DateTimeOffset.Now, fields.Select(f => new LogKeyValue { key = f.Key, value = f.Value }).ToArray());
            return this;
        }

        public virtual ISpan Log(DateTimeOffset timestamp, IEnumerable<KeyValuePair<string, object>> fields)
        {
            _span.Log(timestamp, fields);
            _hooks.OnSpanLog(_span, _operationName, timestamp, fields.Select(f => new LogKeyValue { key = f.Key, value = f.Value }).ToArray());
            return this;
        }

        public virtual ISpan Log(string @event)
        {
            _span.Log(@event);
            _hooks.OnSpanLog(_span, _operationName, DateTimeOffset.Now, new[] { new LogKeyValue { key = nameof(@event), value = @event } });
            return this;
        }

        public virtual ISpan Log(DateTimeOffset timestamp, string @event)
        {
            _span.Log(timestamp, @event);
            _hooks.OnSpanLog(_span, _operationName, timestamp, new[] { new LogKeyValue { key = nameof(@event), value = @event } });
            return this;
        }

        public virtual ISpan SetBaggageItem(string key, string value) { _span.SetBaggageItem(key, value); return this; }

        public virtual ISpan SetOperationName(string operationName)
        {
            _operationName = operationName;
            _span.SetOperationName(operationName);
            return this;
        }

        public virtual ISpan SetTag(string key, string value) { _span.SetTag(key, value); return this; }

        public virtual ISpan SetTag(string key, bool value) { _span.SetTag(key, value); return this; }

        public virtual ISpan SetTag(string key, int value) { _span.SetTag(key, value); return this; }

        public virtual ISpan SetTag(string key, double value) { _span.SetTag(key, value); return this; }

        public virtual ISpan SetTag(BooleanTag tag, bool value) { _span.SetTag(tag, value); return this; }

        public virtual ISpan SetTag(IntOrStringTag tag, string value) { _span.SetTag(tag, value); return this; }

        public virtual ISpan SetTag(IntTag tag, int value) { _span.SetTag(tag, value); return this; }

        public virtual ISpan SetTag(StringTag tag, string value) { _span.SetTag(tag, value); return this; }

        public override string ToString() => _span.ToString();
    }
}
