using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Asya.DependencyInjection.Extensions
{
    public static class AttributeBasedServiceCollectionExtensions
    {
        public static void AddServicesOfType<T>(
          this IServiceCollection serviceCollection,
          params string[] scanAssembliesStartsWith)
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));
            var loadedAssemblies = AssemblyHelper.GetLoadedAssemblies(scanAssembliesStartsWith);
            serviceCollection.AddServicesOfType<T>(loadedAssemblies);
        }

        public static void AddServicesOfType<T>(
          this IServiceCollection serviceCollection,
          Assembly assemblyToBeScanned)
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));
            var assemblyList = (assemblyToBeScanned != null) ? new List<Assembly>()
      {
        assemblyToBeScanned
      } : throw new ArgumentNullException(nameof(assemblyToBeScanned));
            serviceCollection.AddServicesOfType<T>(assemblyList);
        }

        public static void AddServicesOfType<T>(
          this IServiceCollection serviceCollection,
          IEnumerable<Assembly> assembliesToBeScanned)
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));
            if (assembliesToBeScanned == null)
                throw new ArgumentNullException(nameof(assembliesToBeScanned));
            var toBeScanned = assembliesToBeScanned as Assembly[] ?? assembliesToBeScanned.ToArray();
            if (!toBeScanned.Any())
                throw new ArgumentException($"The {assembliesToBeScanned} is empty.", nameof(assembliesToBeScanned));
            var name = typeof(T).Name;
            ServiceLifetime lifetime;
            if (name != "TransientAttribute")
            {
                if (name != "ScopedAttribute")
                {
                    if (name != "SingletonAttribute")
                        throw new ArgumentException("The type " + typeof(T).Name + " is not a valid type in this context.");
                    lifetime = ServiceLifetime.Singleton;
                }
                else
                    lifetime = ServiceLifetime.Scoped;
            }
            else
                lifetime = ServiceLifetime.Transient;
            foreach (var type1 in toBeScanned
                .SelectMany((Func<Assembly, IEnumerable<Type>>)(assembly => assembly.GetTypes()))
                .Where((Func<Type, bool>)(type => type.IsDefined(typeof(T),
                    false)))
                .ToList())
            {
                var serviceType = type1;
                var unused = new List<Type>();
                var source = !serviceType.IsGenericType || !serviceType.IsGenericTypeDefinition
                    ? toBeScanned.SelectMany(a => a.GetTypes())
                        .Where(type => serviceType.IsAssignableFrom(type) && type.IsClass)
                        .ToList()
                    : toBeScanned
                        .SelectMany(
                            a => a.GetTypes())
                        .Where(type =>
                            type.IsGenericType &&
                            type.IsClass &&
                            type.GetInterfaces().Any((Func<Type, bool>)(i =>
                                i.IsGenericType &&
                                i.GetGenericTypeDefinition() == serviceType.GetGenericTypeDefinition())))
                        .ToList();
                if (source.Any())
                {
                    foreach (var type2 in source)
                    {
                        var implementation = type2;
                        var service = !(implementation.IsGenericType && implementation.IsGenericTypeDefinition) || !serviceType.IsGenericType || serviceType.IsGenericTypeDefinition || !serviceType.ContainsGenericParameters ? serviceType : serviceType.GetGenericTypeDefinition();
                        if (!serviceCollection.Any(s => s.ServiceType == service && s.ImplementationType == implementation))
                            serviceCollection.Add(new ServiceDescriptor(service, implementation, lifetime));
                    }
                }
                else if (serviceType.IsClass && !serviceCollection.Any(s => s.ServiceType == serviceType && s.ImplementationType == serviceType))
                    serviceCollection.Add(new ServiceDescriptor(serviceType, serviceType, lifetime));
            }
        }
    }
}
