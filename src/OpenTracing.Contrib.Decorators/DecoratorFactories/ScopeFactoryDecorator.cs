using System;
using System.Collections.Generic;
using System.Text;

namespace OpenTracing.Contrib.Decorators.DecoratorFactories
{
#pragma warning disable S3881 // "IDisposable" should be implemented correctly => the decorator does not have own resources to dispose
    sealed class ScopeFactoryDecorator : IScope
#pragma warning restore S3881 // "IDisposable" should be implemented correctly
    {
        private readonly IScope _scope;
        private readonly InternalSpanDecoratorFactory _spanDecoratorFactory;
        private readonly string _operationName;

        public ScopeFactoryDecorator(IScope scope, InternalSpanDecoratorFactory spanDecoratorFactory, string operationName, bool finishfinishSpanOnDispose)
        {
            _scope = scope;
            _spanDecoratorFactory = spanDecoratorFactory ?? throw new ArgumentNullException(nameof(spanDecoratorFactory));
            _operationName = operationName;
        }

        public ISpan Span => _spanDecoratorFactory(_scope.Span, _operationName);

        public void Dispose() => _scope.Dispose();
    }
}
