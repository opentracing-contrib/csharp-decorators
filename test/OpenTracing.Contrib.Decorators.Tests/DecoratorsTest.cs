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
                .OnSpanStarted((span, operationName) => WriteLine($"Span started: {operationName}"))
                .OnSpanActivated((span, operationName) => WriteLine($"Span activated: {operationName}"))
                .OnSpanFinished((span, operationName) => WriteLine($"Span finished: {operationName}"))
                ;

            var decoratedTracer = builder.Build();

            using (var scope = decoratedTracer.BuildSpan("main").StartActive(false))
            {
                var span = decoratedTracer.BuildSpan("not_active").Start();

                try
                {
                    WriteLine("--> Doing something 1");
                    await Task.Delay(10);
                }
                finally
                {
                    span.Finish();
                }

                using (decoratedTracer.BuildSpan("active_child").StartActive())
                {
                    await Task.Delay(10);
                    WriteLine("--> Doing something 2");
                }

                scope.Span.Finish();
            }

            _outputs.Count.ShouldBe(10);

            _outputs[0].ShouldBe("Span started: main");
            _outputs[1].ShouldBe("Span activated: main");
            _outputs[2].ShouldBe("Span started: not_active");
            _outputs[3].ShouldBe(@"--> Doing something 1");
            _outputs[4].ShouldBe("Span finished: not_active");
            _outputs[5].ShouldBe("Span started: active_child");
            _outputs[6].ShouldBe("Span activated: active_child");
            _outputs[7].ShouldBe(@"--> Doing something 2");
            _outputs[8].ShouldBe("Span finished: active_child");
            _outputs[9].ShouldBe("Span finished: main");

            /*
                Span started: main
                Span activated: main
                Span started: not_active
                --> Doing something 1
                Span finished: not_active
                Span started: active_child
                Span activated: active_child
                --> Doing something 2
                Span finished: active_child
                Span finished: main
            */
        }

        [Fact]
        public async Task StartedWithCallbackDecorator()
        {
            var builder = new TracerDecoratorBuilder(_tracer)
             .OnSpanStartedWithCallback(
                (span, operationName) =>
                {
                    WriteLine($"Span started: {operationName}");
                    return (sp, op) => { WriteLine($"Span finished: {operationName}"); };
                })
                ;

            var decoratedTracer = builder.Build();

            using (var scope = decoratedTracer.BuildSpan("main").StartActive(false))
            {
                var span = decoratedTracer.BuildSpan("not_active").Start();

                try
                {
                    WriteLine("--> Doing something 1");
                    await Task.Delay(10);
                }
                finally
                {
                    span.Finish();
                }

                using (decoratedTracer.BuildSpan("active_child").StartActive())
                {
                    await Task.Delay(10);
                    WriteLine("--> Doing something 2");
                }

                scope.Span.Finish();
            }
        }
    }
}
