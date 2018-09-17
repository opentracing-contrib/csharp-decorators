using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using OpenTracing.Mock;
using Shouldly;
using Xunit;

namespace OpenTracing.Contrib.Decorators.Tests
{
    public class ActiveSpansCounterExample
    {
        [Fact]
        public void Test()
        {
            var originalTracer = new MockTracer();

            var activeSpanCount = 0;

            var builder = new TracerDecoratorBuilder(originalTracer)
                .OnSpanActivated((span, name) => Interlocked.Increment(ref activeSpanCount))
                .OnSpanFinished((span, name) => Interlocked.Decrement(ref activeSpanCount))
                ;

            var tracer = builder.Build();

            activeSpanCount.ShouldBe(0);
            using (tracer.BuildSpan("test").StartActive())
            {
                activeSpanCount.ShouldBe(1);
            }

            activeSpanCount.ShouldBe(0);
            {
                var span = tracer.BuildSpan("test").Start();
                activeSpanCount.ShouldBe(0);
                tracer.ScopeManager.Activate(span, finishSpanOnDispose: false).Dispose();
                activeSpanCount.ShouldBe(1);
                span.Finish();
            }

            activeSpanCount.ShouldBe(0);
            {
                var scope = tracer.BuildSpan("test").StartActive(finishSpanOnDispose: false);
                var span = scope.Span;
                scope.Dispose();
                activeSpanCount.ShouldBe(1);
                span.Finish(DateTimeOffset.Now);
            }

            activeSpanCount.ShouldBe(0);
            {
                ISpan span = null;
                using (var scope = tracer.BuildSpan("test").StartActive(finishSpanOnDispose: false))
                {
                    activeSpanCount.ShouldBe(1);
                    span = scope.Span;
                }
                activeSpanCount.ShouldBe(1);
                span.Finish();
            }

            activeSpanCount.ShouldBe(0);
        }
    }
}
