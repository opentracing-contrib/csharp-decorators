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
        int startedSpanCount = 0;


        ITracer GetDecoratedTracer(string flavor)
        {
            var originalTracer = new MockTracer();
            var builder = new TracerDecoratorBuilder(originalTracer);

            switch (flavor)
            {
                case "StartedtAndFinished":
                    builder
                        .OnSpanStarted((span, name) => Interlocked.Increment(ref startedSpanCount))
                        .OnSpanFinished((span, name) => Interlocked.Decrement(ref startedSpanCount))
                        ;
                    break;

                case "StartedWithCallback":
                    builder
                        .OnSpanStartedWithCallback((span, name) =>
                        {
                            Interlocked.Increment(ref startedSpanCount);
                            return (s, n) => Interlocked.Decrement(ref startedSpanCount);
                        })
                        ;
                    break;

                case "Both":
                    builder
                        .OnSpanStarted((span, name) => Interlocked.Increment(ref startedSpanCount))
                        .OnSpanFinished((span, name) => Interlocked.Decrement(ref startedSpanCount))
                        .OnSpanStartedWithCallback((span, name) =>
                        {
                            Interlocked.Increment(ref startedSpanCount);
                            return (s, n) => Interlocked.Decrement(ref startedSpanCount);
                        })
                        ;
                    break;
                case "Five!!":
                    for (int i = 0; i < 5; i++)
                    {
                        builder
                            .OnSpanStarted((span, name) => Interlocked.Increment(ref startedSpanCount))
                            .OnSpanFinished((span, name) => Interlocked.Decrement(ref startedSpanCount))
                            ;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(flavor), flavor, "Unknown");
            }

            return builder.Build();
        }


        [Theory]
        [InlineData("StartedtAndFinished", 1)]
        [InlineData("StartedWithCallback", 1)]
        [InlineData("Both", 2)]
        [InlineData("Five!!", 5)]
        public void Test(string flavor, int multiplier)
        {
            var tracer = GetDecoratedTracer(flavor);

            startedSpanCount.ShouldBe(0);
            using (tracer.BuildSpan("test").StartActive())
            {
                startedSpanCount.ShouldBe(1 * multiplier);
            }

            startedSpanCount.ShouldBe(0);
            {
                var span = tracer.BuildSpan("test").Start();
                startedSpanCount.ShouldBe(1 * multiplier);
                tracer.ScopeManager.Activate(span, finishSpanOnDispose: false).Dispose();
                startedSpanCount.ShouldBe(1 * multiplier);
                span.Finish();
            }

            startedSpanCount.ShouldBe(0);
            {
                var scope = tracer.BuildSpan("test").StartActive(finishSpanOnDispose: false);
                var span = scope.Span;
                scope.Dispose();
                startedSpanCount.ShouldBe(1 * multiplier);
                span.Finish(DateTimeOffset.Now);
            }

            startedSpanCount.ShouldBe(0);
            {
                ISpan span = null;
                using (var scope = tracer.BuildSpan("test").StartActive(finishSpanOnDispose: false))
                {
                    startedSpanCount.ShouldBe(1 * multiplier);
                    span = scope.Span;
                }
                startedSpanCount.ShouldBe(1 * multiplier);
                span.Finish();
            }

            startedSpanCount.ShouldBe(0);
        }
    }
}
