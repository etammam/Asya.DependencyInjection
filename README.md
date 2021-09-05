Asya Dependency Injection
a lightweight dependency injection assembly scanner

you can simply inject your dependencies with a smart attribute decoration and let the package take of your DI registration for the 4 common injection methods (`Transient`, `Singleton`, `Scoped`, `Hosted`) as following 
let's talk about the injection after 
I have created a simple interface with a single method as `IExample.cs` and implementation as `ExampleManager.cs` as following 

`using Asya.DependencyInjection.Attributes;`
`namespace Asya.DependencyInjection
{
    [Scoped]
    public interface IExample
    {
        void Transaction();
    }
}`

and the implementation 

`using System;`
`namespace Asya.DependencyInjection`
`{`
    `public class ExampleManager: IExample`
    `{`
        `public void Transaction()`
        `{`
            `throw new NotImplementedException();`
        `}`
    `}`
`}`

all you want is to mention the attribute as decoration on top of your interface or class you can use one of the following 

[Scoped], [Singleton], [Transient], [Hosted]

then in your startup class you can simple inject this with single method as 
`service.ScanDependencies();` 
which will scan all services around your internal assembly and register each depended on the decoration attribute
**OR**
you can analysis you code with register each using the `AddServiceOfType<TAttrubite>()`; as following 
`AddServiceOfType<Singleton>();` `AddServiceOfType<Scoped>();` `AddServiceOfType<Transient>();` `AddServiceOfType<Hosted>();` which also will scan your assembly to get dependencies