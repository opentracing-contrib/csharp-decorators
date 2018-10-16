
# OpenTrancing decorators
Provide a way to plug extentions to opentracing instrumentation

[![Build status](https://ci.appveyor.com/api/projects/status/d8oaqdspi9pw16b0/branch/master?svg=true)](https://ci.appveyor.com/project/JromeRx/csharp-decorators/branch/master)
[License](https://img.shields.io/badge/License-Apache_2.0-44cc11.svg)
[![Nuget](https://img.shields.io/nuget/v/OpenTracing.Contrib.Decorators.svg)](https://www.nuget.org/packages/OpenTracing.Contrib.Decorators/)

## Installing

Using NuGet Package Manager Console: `PM> OpenTracing.Contrib.Decorators`

## Usage

In order to decorate some behavior to an existing ITracer implementation, you'll have to encapsulate this one in an other ITracer implementation of your own. This project provide some simple way to do that by using a `TracerDecoratorBuilder` and then add behaviors throught predefined hooks.


```csharp
using OpenTracing.Contrib.Decorators;
```

Build some additional behavior using method hooks
```csharp
var builder = new TracerDecoratorBuilder(_tracer)
    .OnSpanStarted((span, operationName) => WriteLine($"Span started: {operationName}"))
    .OnSpanActivated((span, operationName) => WriteLine($"Span activated: {operationName}"))
    .OnSpanFinished((span, operationName) => WriteLine($"Span finished: {operationName}"))
    ;

var decoratedTracer = builder.Build();
```

Then use the resulting decorated tracer as expected previously
```csharp 
using (var scope = decoratedTracer.BuildSpan("main").StartActive())
{
    WriteLine("--> Doing something");
}
```

The output should be like this one :
```csharp
/*
    Span started: main
    Span activated: main
    --> Doing something 1
    Span finished: main
*/
```
