using System;
using System.Collections.Generic;
using System.Text;
using Jaeger;
using Shouldly;
using Xunit;

namespace OpenTracing.Contrib.Decorators.Tests.Examples
{
    public class JeagerTracerDecoratorExample
    {
        [Fact]
        public void Test()
        {
            // jaeger tracer
            var originalTracer = new Tracer.Builder("Test")
                // DO SOME CONFIGURATION HERE
                .Build();

            var count = 0;
            var tracer = new TracerDecoratorBuilder(originalTracer)
                .OnSpanStarted((span, name) => count++)
                .OnSpanActivated((span, name) => count++)
                .OnSpanFinished((span, name) => count++)
                .Build();

            using (tracer.BuildSpan("test").StartActive())
            {
            }

            count.ShouldBe(3);
        }
    }
}
