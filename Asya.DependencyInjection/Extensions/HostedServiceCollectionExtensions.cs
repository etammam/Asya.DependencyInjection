using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Asya.DependencyInjection.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Asya.DependencyInjection.Extensions
{
    public static class HostedServiceCollectionExtensions
    {
        public static void AddHostedServices(
            this IServiceCollection serviceCollection,
            params string[] scanAssembliesStartsWith)
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));
            var loadedAssemblies = AssemblyHelper.GetLoadedAssemblies(scanAssembliesStartsWith);
            serviceCollection.AddHostedServices(loadedAssemblies);
        }

        public static void AddHostedServices(
            this IServiceCollection serviceCollection,
            Assembly assemblyToBeScanned)
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));
            List<Assembly> assemblyList = new List<Assembly>()
            {
                assemblyToBeScanned
            };
            serviceCollection.AddHostedServices(assemblyList);
        }

        public static void AddHostedServices(
            this IServiceCollection serviceCollection,
            IEnumerable<Assembly> assembliesToBeScanned)
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));
            foreach (var implementationType in assembliesToBeScanned
                .SelectMany(
                    (Func<Assembly, IEnumerable<Type>>)(assembly => assembly.GetTypes()))
                .Where((Func<Type, bool>)(type => type.IsDefined(typeof(HostedAttribute),
                    false)))
                .ToList())
                serviceCollection.TryAddEnumerable(new ServiceDescriptor(typeof(IHostedService), implementationType, ServiceLifetime.Singleton));
        }
    }
}
