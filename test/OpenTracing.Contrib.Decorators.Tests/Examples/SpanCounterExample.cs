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
        public static long CurrentActiveSpanCount;

        class CurrentActiveSpanDecorator : SpanDecorator
        {
            public CurrentActiveSpanDecorator(ISpan span) : base(span)
            {
            }

            public override void Finish()
            {
                Interlocked.Decrement(ref CurrentActiveSpanCount);
                base.Finish();
            }

            public override void Finish(DateTimeOffset finishTimestamp)
            {
                Interlocked.Decrement(ref CurrentActiveSpanCount);
                base.Finish(finishTimestamp);
            }
        }

        class CurrentActiveSpanBuilderDecorator : SpanBuilderDecorator
        {
            public CurrentActiveSpanBuilderDecorator(ISpanBuilder spanBuilder) : base(spanBuilder)
            {
            }

            public override ISpan Start()
            {
                Interlocked.Increment(ref CurrentActiveSpanCount);
                return base.Start();
            }

            public override IScope StartActive()
            {
                Interlocked.Increment(ref CurrentActiveSpanCount);
                return base.StartActive();
            }

            public override IScope StartActive(bool finishSpanOnDispose)
            {
                Interlocked.Increment(ref CurrentActiveSpanCount);
                return base.StartActive(finishSpanOnDispose);
            }
        }


        class CurrentActiveScopeDecorator : ScopeDecorator
        {
            public CurrentActiveScopeDecorator(IScope scope) : base(scope)
            {
            }

            public override void Dispose()
            {
                Interlocked.Decrement(ref CurrentActiveSpanCount);
                base.Dispose();
            }
        }

        [Fact]
        public void Test()
        {
            var originalTracer = new MockTracer();

            var builder = new TracerDecoratorBuilder(originalTracer)
                .WithScopeDecorator(scope => new CurrentActiveScopeDecorator(scope))
                .WithSpanDecorator(span => new CurrentActiveSpanDecorator(span))
                .WithSpanBuilderDecorator(spanBuilder => new CurrentActiveSpanBuilderDecorator(spanBuilder))
                ;

            var tracer = builder.Build();


            CurrentActiveSpanCount.ShouldBe(0);
            using (tracer.BuildSpan("test").StartActive())
            {
                CurrentActiveSpanCount.ShouldBe(1);
            }

            CurrentActiveSpanCount.ShouldBe(0);
            {
                var span = tracer.BuildSpan("test").Start();
                CurrentActiveSpanCount.ShouldBe(1);
                span.Finish();
            }

            CurrentActiveSpanCount.ShouldBe(0);
            {
                var span = tracer.BuildSpan("test").Start();
                CurrentActiveSpanCount.ShouldBe(1);
                span.Finish(DateTimeOffset.Now);
            }

            CurrentActiveSpanCount.ShouldBe(0);
            {
                ISpan span = null;
                using (var scope = tracer.BuildSpan("test").StartActive(finishSpanOnDispose: false))
                {
                    CurrentActiveSpanCount.ShouldBe(1);
                    span = scope.Span;
                }
                CurrentActiveSpanCount.ShouldBe(1);
                span.Finish();
            }

            CurrentActiveSpanCount.ShouldBe(0);
        }
    }
}
