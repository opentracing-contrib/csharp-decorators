using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task WriteSomeSpanStory()
        {
            var builder = new TracerDecoratorBuilder(_tracer)
                .OnSpanStarted((span, operationName) => WriteLine($"Started: {operationName}"))
                .OnSpanActivated((span, operationName) => WriteLine($"Activated: {operationName}"))
                .OnSpanFinished((span, operationName) => WriteLine($"Finished: {operationName}"))
                .OnSpanLog((span, operationName, timestamp, fields) =>
                {
                    WriteLine($"Log: {operationName} : {string.Join(", ", fields.Select(f => $"{f.key}/{f.value}"))}");
                })
                .OnSpanSetTag((span, operationName, tag) =>
                {
                    WriteLine($"Tag: {operationName} : {tag.key}/{tag.value}");
                });
            ;

            var decoratedTracer = builder.Build();

            using (var scope = decoratedTracer
                .BuildSpan("main")
                .WithTag("tag1", 1)
                .StartActive(false)
                )
            {
                scope.Span.SetTag("tag2", 2);

                var span = decoratedTracer.BuildSpan("not_active").Start();

                try
                {
                    WriteLine("--> Doing something 1");
                    span.SetTag("tag3", true);
                    await Task.Delay(10);
                }
                finally
                {
                    span.Finish();
                }

                using (decoratedTracer.BuildSpan("active_child").StartActive())
                {
                    decoratedTracer.ActiveSpan.Log(new Dictionary<string, object> { { "log1", 42 }, { "log2", "test" } });
                    await Task.Delay(10);
                    WriteLine("--> Doing something 2");
                }

                scope.Span.Finish();
            }

            var lines = new[]
            {
                "Started: main",
                "Tag: main : tag1/1",
                "Activated: main",
                "Tag: main : tag2/2",
                "Started: not_active",
                @"--> Doing something 1",
                "Tag: not_active : tag3/True",
                "Finished: not_active",
                "Started: active_child",
                "Activated: active_child",
                "Log: active_child : log1/42, log2/test",
                @"--> Doing something 2",
                "Finished: active_child",
                "Finished: main",
            };

            _outputs.Count.ShouldBe(lines.Length);
            for (int i = 0; i < lines.Length; i++)
            {
                _outputs[i].ShouldBe(lines[i]);
            }
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
