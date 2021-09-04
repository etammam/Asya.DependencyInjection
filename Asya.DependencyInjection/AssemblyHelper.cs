using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Asya.DependencyInjection
{
    internal static class AssemblyHelper
    {
        private static List<Assembly> _loadedAssemblies = new();

        public static List<Assembly> GetLoadedAssemblies(
          params string[] scanAssembliesStartsWith)
        {
            if (_loadedAssemblies.Any())
                return _loadedAssemblies;
            LoadAssemblies(scanAssembliesStartsWith);
            return _loadedAssemblies;
        }

        private static void LoadAssemblies(params string[] scanAssembliesStartsWith)
        {
            var source = new HashSet<Assembly>();
            var stringList = new List<string>();
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (scanAssembliesStartsWith != null && scanAssembliesStartsWith.Any())
            {
                if (scanAssembliesStartsWith.Length == 1)
                {
                    var searchPattern = scanAssembliesStartsWith.First() + "*.dll";
                    var files = Directory.GetFiles(baseDirectory, searchPattern, SearchOption.AllDirectories);
                    stringList.AddRange(files);
                }
                if (scanAssembliesStartsWith.Length > 1)
                {
                    foreach (var str in scanAssembliesStartsWith)
                    {
                        var searchPattern = str + "*.dll";
                        var files = Directory.GetFiles(baseDirectory, searchPattern, SearchOption.AllDirectories);
                        stringList.AddRange(files);
                    }
                }
            }
            else
            {
                var files = Directory.GetFiles(baseDirectory, "*.dll");
                stringList.AddRange(files);
            }
            foreach (var assemblyFile in stringList)
            {
                var assembly = Assembly.LoadFrom(assemblyFile);
                source.Add(assembly);
            }
            _loadedAssemblies = source.ToList();
        }
    }
}