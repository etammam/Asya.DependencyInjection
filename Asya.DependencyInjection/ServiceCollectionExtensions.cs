using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Asya.DependencyInjection.Attributes;
using Asya.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Asya.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddServicesOfAllTypes(
            this IServiceCollection serviceCollection,
            params string[] scanAssembliesStartsWith)
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));
            var loadedAssemblies = AssemblyHelper.GetLoadedAssemblies(scanAssembliesStartsWith);
            serviceCollection.AddServicesOfAllTypes(loadedAssemblies);
        }

        public static void AddServicesOfAllTypes(
            this IServiceCollection serviceCollection,
            Assembly assemblyToBeScanned)
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));
            var assemblyList = (assemblyToBeScanned != null) ? new List<Assembly>()
            {
                assemblyToBeScanned
            } : throw new ArgumentNullException(nameof(assemblyToBeScanned));
            serviceCollection.AddServicesOfAllTypes(assemblyList);
        }

        public static void AddServicesOfAllTypes(
            this IServiceCollection serviceCollection,
            IEnumerable<Assembly> assembliesToBeScanned)
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));
            if (assembliesToBeScanned == null)
                throw new ArgumentNullException(nameof(assembliesToBeScanned));
            var toBeScanned = assembliesToBeScanned as Assembly[] ?? assembliesToBeScanned.ToArray();
            serviceCollection.AddServicesOfType<TransientAttribute>(toBeScanned);
            serviceCollection.AddServicesOfType<ScopedAttribute>(toBeScanned);
            serviceCollection.AddServicesOfType<SingletonAttribute>(toBeScanned);
            serviceCollection.AddHostedServices(toBeScanned);
        }
    }
}
