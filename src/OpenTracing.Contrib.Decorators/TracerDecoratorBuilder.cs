using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenTracing.Contrib.Decorators
{
    public class TracerDecoratorBuilder
    {
        private readonly ITracer _tracer;
        public TracerDecoratorBuilder(ITracer tracer)
        {
            _tracer = tracer;
        }

        readonly List<OnSpanStarted> _onSpanStarteds = new List<OnSpanStarted>();
        public TracerDecoratorBuilder OnSpanStarted(OnSpanStarted onSpanStarted)
        {
            _onSpanStarteds.Add(onSpanStarted);
            return this;
        }

        readonly List<OnSpanStartedWithFinishCallback> _onSpanStartedWithCallbacks = new List<OnSpanStartedWithFinishCallback>();
        public TracerDecoratorBuilder OnSpanStartedWithCallback(OnSpanStartedWithFinishCallback onSpanStartedWithFinishCallback)
        {
            _onSpanStartedWithCallbacks.Add(onSpanStartedWithFinishCallback);
            return this;
        }

        readonly List<OnSpanActivated> _onSpanActivateds = new List<OnSpanActivated>();
        public TracerDecoratorBuilder OnSpanActivated(OnSpanActivated onSpanActivated)
        {
            _onSpanActivateds.Add(onSpanActivated);
            return this;
        }

        readonly List<OnSpanFinished> _onSpanFinisheds = new List<OnSpanFinished>();
        public TracerDecoratorBuilder OnSpanFinished(OnSpanFinished onSpanFinisheds)
        {
            _onSpanFinisheds.Add(onSpanFinisheds);
            return this;
        }

        public ITracer Build()
        {
            var hooks = new BuildersDecoratorHooks();

            if (_onSpanStarteds.Count != 0)
            {
                var delegates = _onSpanStarteds.ToArray();
                hooks.OnSpanStarted = (span, operationName) =>
                {
                    foreach (var d in delegates)
                        d(span, operationName);
                };
            }

            if (_onSpanActivateds.Count != 0)
            {
                var delegates = _onSpanActivateds.ToArray();
                hooks.OnSpanActivated = (span, operationName) =>
                {
                    foreach (var d in delegates)
                        d(span, operationName);
                };
            }

            if (_onSpanFinisheds.Count != 0)
            {
                var delegates = _onSpanFinisheds.ToArray();
                hooks.OnSpanFinished = (span, operationName) =>
                {
                    foreach (var d in delegates)
                        d(span, operationName);
                };
            }

            if (_onSpanStartedWithCallbacks.Count != 0)
            {
                var delegates = _onSpanStartedWithCallbacks.ToArray();
                hooks.OnSpanStartedWithFinishCallback = (span, operationName) =>
                {
                    var callBacks = delegates.Select(d => d(span, operationName)).ToArray();
                    return (sp, op) =>
                    {
                        foreach (var callBack in callBacks)
                        {
                            callBack(sp, op);
                        }
                    };
                };
            }

            return new TracerDecorator(_tracer, hooks);
        }


    }
}
