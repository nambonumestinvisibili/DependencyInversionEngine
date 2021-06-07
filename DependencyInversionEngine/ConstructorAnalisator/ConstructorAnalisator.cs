using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DependencyInversionEngine.ConstructorAnalyzer
{
    internal class ConstructorAnalisator
    {
        public IEnumerable<ConstructorInfo> GetConstructorsWithMaximalNoOfParameters(Type type)
        {
            var constructors = new List<ConstructorInfo>(type.GetConstructors());

            if (constructors.Count() == 0) throw new NoConstructorsException(String.Format("No constructors {0}", type));

            var maxNumberOfParms = constructors
                .Select(x => x.GetParameters().Length).Max();
            var constructorsWithMaxParams = constructors
                .Where(x => x.GetParameters().Length == maxNumberOfParms).ToList();

            return constructorsWithMaxParams;
        }

        public IEnumerable<ConstructorInfo> GetConstructors(Type type)
        {
            List<ConstructorInfo> attributedConstructors = new List<ConstructorInfo>();

            foreach (var constructor in type.GetConstructors())
            {
                if (constructor
                    .GetCustomAttributes(typeof(DependencyConstructor), false)
                    .Any())
                {
                    attributedConstructors.Add(constructor);
                }
            }

            if (attributedConstructors.Count() == 0)
            {
                attributedConstructors = GetConstructorsWithMaximalNoOfParameters(type).ToList();
            }

            return attributedConstructors;
        }
    }
}
