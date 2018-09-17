using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTracing.Contrib.Decorators.DecoratorFactories
{
    delegate IScopeManager InternalScopeManagerDecoratorFactory(IScopeManager scopeManager, string operationName);
    delegate ISpan InternalSpanDecoratorFactory(ISpan span, string operationName);
    delegate ISpanBuilder InternalSpanBuilderDecoratorFactory(ISpanBuilder spanBuilder, string operationName);
    delegate ISpanContext InternalSpanContextDecoratorFactory(ISpanContext spanContext);
    delegate IScope InternalScopeDecoratorFactory(IScope scope, string operationName, bool finishSpanOnDispose = true);

    static class Defaults
    {
       public const string UNKNOWN_OPERATION_NAME = "UNKNOWN";
    }

}
