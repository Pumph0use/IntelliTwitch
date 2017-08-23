using System.Collections.Generic;
using System;
using System.Linq;

namespace IntelligentMachine
{
    public static class Utilities
    {
        public static T[] BuildClassArrayFromInterface<T>() where T : class
        {
            var result = new List<T>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.GetInterfaces().Contains(typeof(T)) && type.IsClass)
                        result.Add((T)Activator.CreateInstance(type));
                }
            }

            return result.ToArray();
        }

    }
}

