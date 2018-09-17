using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using OpenTracing.Contrib.Decorators;
using OpenTracing.Mock;
using OpenTracing.Noop;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace OpenTracing.Contrib.Decorators.Tests
{
    public class DecoratorsTest
    {
        private readonly ITracer _tracer = new MockTracer();
        private readonly ITestOutputHelper _output;
        private readonly List<string> _outputs = new List<string>();

        public DecoratorsTest(ITestOutputHelper output)
        {
            _output = output;
        }

        void WriteLine(string line)
        {
            _output.WriteLine(line);
            _outputs.Add(line);
        }

        [Fact]
        public async Task Test()
        {
            var builder = new TracerDecoratorBuilder(_tracer)
                .OnSpanStarted((span, operationName) => WriteLine($"Span started : {span}"))
                .OnSpanActivated((span, operationName) => WriteLine($"Span activated : {span}"))
                .OnSpanFinished((span, operationName) => WriteLine($"Span finished : {span}"))
                ;

            var sut = builder.Build();

            using (var scope = sut.BuildSpan("main").StartActive(false))
            {
                var span = sut.BuildSpan("not_active").Start();

                try
                {
                    WriteLine("--> Doing something 1");
                    await Task.Delay(10);
                }
                finally
                {
                    span.Finish();
                }

                using (sut.BuildSpan("active_child").StartActive())
                {
                    await Task.Delay(10);
                    WriteLine("--> Doing something 2");
                }

                scope.Span.Finish();
            }

            // IDs can changed from one to another execution, only testing start & end of the outputs
            _outputs.Count.ShouldBe(10);

            _outputs[0].ShouldStartWith("Span started");
            _outputs[0].ShouldEndWith("OperationName: main");
            _outputs[1].ShouldStartWith("Span activated");
            _outputs[1].ShouldEndWith("OperationName: main");
            _outputs[2].ShouldStartWith("Span started");
            _outputs[2].ShouldEndWith("OperationName: not_active");

            _outputs[3].ShouldBe(@"--> Doing something 1");

            _outputs[4].ShouldStartWith("Span finished");
            _outputs[4].ShouldEndWith("OperationName: not_active");
            _outputs[5].ShouldStartWith("Span started");
            _outputs[5].ShouldEndWith("OperationName: active_child");
            _outputs[6].ShouldStartWith("Span activated");
            _outputs[6].ShouldEndWith("OperationName: active_child");

            _outputs[7].ShouldBe(@"--> Doing something 2");

            _outputs[8].ShouldStartWith("Span finished");
            _outputs[8].ShouldEndWith("OperationName: active_child");
            _outputs[9].ShouldStartWith("Span finished");
            _outputs[9].ShouldEndWith("OperationName: main");

            /*
                Span started : TraceId: 1, SpanId: 2, ParentId: , OperationName: main
                Span activated : TraceId: 1, SpanId: 2, ParentId: , OperationName: main
                Span started : TraceId: 1, SpanId: 3, ParentId: 2, OperationName: not_active
                --> Doing something 1
                Span finished : TraceId: 1, SpanId: 3, ParentId: 2, OperationName: not_active
                Span started : TraceId: 1, SpanId: 4, ParentId: 2, OperationName: active_child
                Span activated : TraceId: 1, SpanId: 4, ParentId: 2, OperationName: active_child
                --> Doing something 2
                Span finished : TraceId: 1, SpanId: 4, ParentId: 2, OperationName: active_child
                Span finished : TraceId: 1, SpanId: 2, ParentId: , OperationName: main
            */
        }
    }
}
