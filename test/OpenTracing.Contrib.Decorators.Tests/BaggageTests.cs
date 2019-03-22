using System;
using System.Collections.Generic;
using System.Text;
using OpenTracing.Mock;
using Shouldly;
using Xunit;

namespace OpenTracing.Contrib.Decorators.Tests
{
    /// <summary>
    /// ported from https://github.com/ndrwrbgs/OpenTracing-EventHookTracer/blob/80d36340793fecb9109b83131c1dc6406a08370e/test/Library.Tests/EventHookTests.cs#L15
    /// </summary>
    public class BaggageTests
    {
        private readonly ITracer _tracer = new MockTracer();
        private readonly ITracer _decoratedTracer;

        public BaggageTests()
        {
            _decoratedTracer = new TracerDecoratorBuilder(_tracer).Build();
        }

        [Fact]
        public void BaggageOnSameItem()
        {
            var scope = _decoratedTracer.BuildSpan("1")
                .StartActive();
            scope.Span.SetBaggageItem("key", "value");


            scope.Span.GetBaggageItem("key").ShouldBe("value");
        }

        [Fact]
        public void BaggagePropagatesToChildren()
        {
            using (var scope = _decoratedTracer.BuildSpan("1")
                .StartActive())
            {
                scope.Span.SetBaggageItem("key", "value");

                using (var child = _decoratedTracer.BuildSpan("2")
                    .StartActive())
                {
                    child.Span.GetBaggageItem("key").ShouldBe("value");
                }
            }
        }


        [Fact]
        public void ChildDoesNotAffectParent()
        {
            var scope = _decoratedTracer.BuildSpan("1")
                .StartActive();

            var child = _decoratedTracer.BuildSpan("2")
                .StartActive();
            child.Span.SetBaggageItem("key", "value");

            scope.Span.GetBaggageItem("key").ShouldBeNull();
        }
    }

}
