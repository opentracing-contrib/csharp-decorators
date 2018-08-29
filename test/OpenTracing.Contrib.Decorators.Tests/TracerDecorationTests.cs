using System;
using System.Collections.Generic;
using System.Text;
using OpenTracing.Mock;
using Shouldly;
using Xunit;

namespace OpenTracing.Contrib.Decorators.Tests
{
    public class TracerDecorationTests
    {
        class MySpanDecorator : SpanDecorator
        {
            public bool Finished { get; set; }

            public MySpanDecorator(ISpan span) : base(span) { }
            public override void Finish()
            {
                Finished = true;
                base.Finish();
            }
        }

        [Fact]
        public void Should_decorate_span_without_tracer_decorator()
        {
            var builder = new TracerDecoratorBuilder(new MockTracer());

            MySpanDecorator mySpanDecorator = null;

            builder.WithSpanDecorator(sp =>
            {
                mySpanDecorator = new MySpanDecorator(sp);
                return mySpanDecorator;
            });

            var tracer = builder.Build();
            var span = tracer.BuildSpan("test")
                .Start();

            mySpanDecorator.ShouldNotBeNull();
            mySpanDecorator.Finished.ShouldBeFalse();
            span.Finish();
            mySpanDecorator.Finished.ShouldBeTrue();
        }
    }
}
