using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;

namespace DependencyInversionEngine
{
    public interface IInstanceProvider
    {
        public object Create(Dictionary<Type, IInstanceProvider> registeredTypes);
    }

    public abstract class AbstractInstanceProvider : IInstanceProvider
    {
        public abstract object Create(Dictionary<Type, IInstanceProvider> registeredTypes);

        protected  Type _type;

        public AbstractInstanceProvider(Type type)
        {
            _type = type; 
        }
        public AbstractInstanceProvider()
        {

        }

        

        protected object Resolve(Dictionary<Type, IInstanceProvider> registeredTypes)
        {
            var resolvedParams = new List<object>();

            var constructorsInfo = GetConstructorsWithMaximalNoOfParameters();

            if (constructorsInfo.ElementAt(0).GetParameters().Length == 0)
            {
                return constructorsInfo.ElementAt(0).Invoke(resolvedParams.ToArray());
            }

            foreach (var constructorInfo in constructorsInfo)
            {
                try
                {
                    resolvedParams = ResolveConstructor(constructorInfo, registeredTypes);
                }
                catch (Exception e)
                {
                    resolvedParams.Clear();
                    continue;
                }

                var instance = constructorInfo.Invoke(resolvedParams.ToArray());
                return instance;
            }

            throw new Exception("Type could not be resolved");
        }

        private List<object> ResolveConstructor(
            ConstructorInfo constructorInfo,
            Dictionary<Type, IInstanceProvider> registeredTypes 
            )
        {
            var resolvedParameters = new List<object>(); 

            foreach (var paramInfo in constructorInfo.GetParameters())
            {
                try
                {
                    var parameter = ResolveParameter(paramInfo.ParameterType, registeredTypes);
                    resolvedParameters.Add(parameter);
                }
                catch (Exception e)
                {
                    throw new Exception("Constructor cannot be resolved");
                }
            }

            return resolvedParameters;
        }

        private object ResolveParameter(
            Type type, 
            Dictionary<Type, IInstanceProvider> registeredTypes)
        {
           if (registeredTypes.ContainsKey(type))
            {
                return registeredTypes[type].Create(registeredTypes);
            }
           else
            {
                throw new Exception("Parameter has not been registered");
            }
        }

        protected IEnumerable<ConstructorInfo> GetConstructorsWithMaximalNoOfParameters()
        {
            var constructors = new List<ConstructorInfo>(_type.GetConstructors());

            var maxNumberOfParms = constructors
                .Select(x => x.GetParameters().Length).Max();
            var constructorsWithMaxParams = constructors
                .Where(x => x.GetParameters().Length == maxNumberOfParms).ToList();

            return constructorsWithMaxParams;
        }
    }
}





















