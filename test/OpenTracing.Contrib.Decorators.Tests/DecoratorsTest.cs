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

        class TestOutputTracerDecorator : TracerDecorator
        {
            private readonly Action<string> _output;

            public TestOutputTracerDecorator(ITracer tracer, Action<string> output) : base(tracer)
            {
                _output = output;
            }

            public override ISpanBuilder BuildSpan(string operationName)
            {
                _output($"Building span named {operationName}");
                return base.BuildSpan(operationName);
            }
        }

        class TestOutputSpanBuilderDecorator : SpanBuilderDecorator
        {
            private readonly Action<string> _output;

            public TestOutputSpanBuilderDecorator(ISpanBuilder spanBuilder, Action<string> output) : base(spanBuilder)
            {
                _output = output;
            }

            public override ISpan Start()
            {
                var span = base.Start();
                _output($"Span Started: {span}");
                return span;
            }

            public override IScope StartActive()
            {
                var scope = base.StartActive();
                _output($"Scope Started: {scope.Span}");
                return scope;
            }

            public override IScope StartActive(bool finishSpanOnDispose)
            {
                var scope = base.StartActive(finishSpanOnDispose);
                _output($"Scope Started: {scope.Span}");
                return scope;
            }
        }

        class TestOutputScopeDecorator : ScopeDecorator
        {
            private readonly Action<string> _output;

            public TestOutputScopeDecorator(IScope scope, Action<string> output) : base(scope)
            {
                _output = output;
            }

            public override void Dispose()
            {
                base.Dispose();
                _output($"Scope disposed: {Span}");
            }
        }

        class TestOutputSpanDecorator : SpanDecorator
        {
            private readonly Action<string> _output;

            public TestOutputSpanDecorator(ISpan span, Action<string> output) : base(span)
            {
                _output = output;
            }

            public override void Finish()
            {
                base.Finish();
                _output($"Span Finished: {this}");
            }
        }

        [Fact]
        public async Task Test()
        {
            var builder = new TracerDecoratorBuilder(_tracer)
                .WithTracerDecorator(tracer => new TestOutputTracerDecorator(tracer, WriteLine))
                .WithSpanBuilderDecorator(spanBuilder => new TestOutputSpanBuilderDecorator(spanBuilder, WriteLine))
                .WithScopeDecorator(scope => new TestOutputScopeDecorator(scope, WriteLine))
                .WithSpanDecorator(span => new TestOutputSpanDecorator(span, WriteLine))
                ;

            var sut = builder.Build();

            using (var scope = sut.BuildSpan("StartActive(false)").StartActive(false))
            {
                var span = sut.BuildSpan("Start()").Start();

                try
                {
                    WriteLine("--> Doing something 1");
                    await Task.Delay(10);
                }
                finally
                {
                    span.Finish();
                }

                using (sut.BuildSpan("StartActive()").StartActive())
                {
                    await Task.Delay(10);
                    WriteLine("--> Doing something 2");
                }

                scope.Span.Finish();
            }

            // Asserting only lines with to IDs
            _outputs.Count.ShouldBe(12);
            _outputs[0].ShouldBe(@"Building span named StartActive(false)");
            _outputs[2].ShouldBe(@"Building span named Start()");
            _outputs[4].ShouldBe(@"--> Doing something 1");
            _outputs[5].ShouldBe(@"Span Finished: OpenTracing.Contrib.Decorators.Tests.DecoratorsTest+TestOutputSpanDecorator");
            _outputs[6].ShouldBe(@"Building span named StartActive()");
            _outputs[8].ShouldBe(@"--> Doing something 2");
            _outputs[10].ShouldBe(@"Span Finished: OpenTracing.Contrib.Decorators.Tests.DecoratorsTest+TestOutputSpanDecorator");

            /*
                Building span named StartActive(false)
                Scope Started: TraceId: 1, SpanId: 2, ParentId: , OperationName: StartActive(false)
                Building span named Start()
                Span Started: TraceId: 1, SpanId: 3, ParentId: 2, OperationName: Start()
                --> Doing something 1
                Span Finished: OpenTracing.Contrib.Decorators.Tests.DecoratorsTest+TestOutputSpanDecorator
                Building span named StartActive()
                Scope Started: TraceId: 1, SpanId: 4, ParentId: 2, OperationName: StartActive()
                --> Doing something 2
                Scope disposed: TraceId: 1, SpanId: 4, ParentId: 2, OperationName: StartActive()
                Span Finished: OpenTracing.Contrib.Decorators.Tests.DecoratorsTest+TestOutputSpanDecorator
                Scope disposed: TraceId: 1, SpanId: 2, ParentId: , OperationName: StartActive(false)
            */
        }
    }
}
