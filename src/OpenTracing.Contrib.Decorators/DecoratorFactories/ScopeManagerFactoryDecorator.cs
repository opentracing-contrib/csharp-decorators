using System;
using System.Collections.Generic;
using System.Text;

namespace OpenTracing.Contrib.Decorators.DecoratorFactories
{
    sealed class ScopeManagerFactoryDecorator : IScopeManager
    {
        private readonly IScopeManager _scopeManager;
        private readonly InternalScopeDecoratorFactory _scopeDecoratorFactory;
        private readonly string _operationName;

        public ScopeManagerFactoryDecorator(IScopeManager scopeManager, InternalScopeDecoratorFactory scopeDecoratorFactory, string operationName)
        {
            _scopeManager = scopeManager;
            _scopeDecoratorFactory = scopeDecoratorFactory ?? throw new ArgumentNullException(nameof(scopeDecoratorFactory));
            _operationName = operationName;
        }

        public IScope Active => _scopeDecoratorFactory(_scopeManager.Active, _operationName);

        public IScope Activate(ISpan span, bool finishSpanOnDispose) => _scopeDecoratorFactory(_scopeManager.Activate(span, finishSpanOnDispose), _operationName, finishSpanOnDispose);
    }
}
