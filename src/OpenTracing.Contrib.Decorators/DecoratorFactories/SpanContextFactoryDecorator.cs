using System;
using System.Collections.Generic;
using System.Text;

namespace OpenTracing.Contrib.Decorators
{
    sealed class SpanContextFactoryDecorator : ISpanContext
    {
        private readonly ISpanContext _spanContext;

        public SpanContextFactoryDecorator(ISpanContext spanContext)
        {
            _spanContext = spanContext;
        }

        public string TraceId => _spanContext.TraceId;

        public string SpanId => _spanContext.SpanId;

        public IEnumerable<KeyValuePair<string, string>> GetBaggageItems() => _spanContext.GetBaggageItems();
    }
}
