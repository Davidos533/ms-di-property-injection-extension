<p align="center">
  <a href="#">
    <img height="128" width="128" src="https://raw.githubusercontent.com/Davidos533/ms-di-property-injection-extension/master/icon.png">
  </a>
</p>

<h1 align="center">
  MS DI Property Injection
</h1>

<p align="center">
  <a href="https://www.nuget.org/packages/DJMJ.Extensions.DependencyInjection.Property/">
    <img alt="Nuget" src="https://img.shields.io/nuget/v/DJMJ.Extensions.DependencyInjection.Property">
  </a>
</p>


## Usage
### Mark property for injection

```csharp
using Microsoft.Extensions.DependencyInjection;

public class FooService : IFooService
{
    [Inject]
    public IBooService BooService { get; set; }

    public void Foo()
    {
        // just start using injected property
        BooService...
    }
}
```
### Add services scan method in ConfigureServices

```csharp
using Microsoft.Extensions.DependencyInjection;

...

 host.ConfigureServices((services)=>
            {               
                services.AddTransient<IBooService, BooService>();
                services.AddTransient<IFooService, FooService>();

                // scan method
                services.AddPropertyInjectedServices();
            });
```
#
## Install

```
Install-Package DJMJ.Extensions.DependencyInjection.Property
```
