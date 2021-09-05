using Asya.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Asya.DependencyInjection
{
    public static class Injector
    {
        public static IServiceCollection ScanDependencies(this IServiceCollection service)
        {
            service.AddServicesOfAllTypes();
            return service;
        }
    }
}
