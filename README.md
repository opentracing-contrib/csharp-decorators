
# OpenTrancing decorators
Provide a way to plug extentions to opentracing instrumentation

[![Build status](https://ci.appveyor.com/api/projects/status/d8oaqdspi9pw16b0/branch/master?svg=true)](https://ci.appveyor.com/project/JromeRx/csharp-decorators/branch/master)
![License](https://img.shields.io/badge/License-Apache_2.0-44cc11.svg)
![Nuget](https://img.shields.io/nuget/v/OpenTracing.Contrib.Decorators.svg)

## Installing

Using NuGet Package Manager Console:
`PM> OpenTracing.Contrib.Decorators`

## Usage

In order to decorate some behavior to an existing ITracer implementation, you'll have to encapsulate this one in an other ITracer implementation of your own. This project provide some simple way to do that by using a `TracerDecoratorBuilder` and then define some decorators `ITracer, IScopeManager, ISpan, ISpanBuilder, ISpanContext or IScope`.


```csharp
using OpenTracing.Contrib.Decorators;
```

Build some additional behavior using method hooks
```csharp
// TODO
```

Then use the tracer as expected previously
```csharp 
// TODO
```

Assertions
```csharp
// TODO
```
