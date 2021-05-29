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

            var constructorsInfo = GetConstructors(_type);

            var info = "";

            foreach (var constructorInfo in constructorsInfo)
            {

                try
                {
                    resolvedParams = ResolveConstructor(constructorInfo, registeredTypes);
                }
                catch (ConstructorUnresolvableException e)
                {
                    resolvedParams.Clear();
                    info = " - " + e.Message;
                    continue;
                }
                

                var instance = constructorInfo.Invoke(resolvedParams.ToArray());
                return instance;
            }

            throw new ConstructorUnresolvableException("Type could not be resolved" + info);
        }

        private List<object> ResolveConstructor(
            ConstructorInfo constructorInfo,
            Dictionary<Type, IInstanceProvider> registeredTypes
            )
        {
            if (CycleDetected(constructorInfo, _type, registeredTypes)) {
                throw new DependencyCycleException("Dependency circuit has been detected");
            }
            var resolvedParameters = new List<object>(); 

            foreach (var paramInfo in constructorInfo.GetParameters())
            {
                try
                {
                    var parameter = ResolveParameter(paramInfo.ParameterType, registeredTypes);
                    resolvedParameters.Add(parameter);
                }
                catch (UnregisteredParameterException e)
                {
                    throw new ConstructorUnresolvableException("Constructor cannot be resolved - " + e.Message);
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
                throw new UnregisteredParameterException(String.Format("Parameter {0} has not been registered", type));
            }
        }

        private bool CycleDetected(
            ConstructorInfo constructorInfo,
            Type rootType, 
            Dictionary<Type, IInstanceProvider> registeredTypes)
        {
            var parameters = constructorInfo.GetParameters();
            var noneOfTheParametersHaveDependencyOnRootType = 
                parameters.All(x => x.ParameterType != rootType);
            if (!noneOfTheParametersHaveDependencyOnRootType) return true;

            var noDependencyFurther = parameters.All(x =>
            {
                if (registeredTypes.ContainsKey(x.ParameterType) && registeredTypes[x.ParameterType].GetType() == typeof(SingletonProvider))
                {
                    return true;
                }
                var construcotrs = GetConstructorsWithMaximalNoOfParameters(x.ParameterType);
                var res = true;
                construcotrs.ToList().ForEach(c =>
                {
                    res = res && c.GetParameters().All(z => !CycleDetected(c, rootType, registeredTypes));
                });
                return res;
            });
            return !(noneOfTheParametersHaveDependencyOnRootType && noDependencyFurther);
        }

        private IEnumerable<ConstructorInfo> GetConstructorsWithMaximalNoOfParameters(Type type)
        {
            var constructors = new List<ConstructorInfo>(type.GetConstructors());

            if (constructors.Count() == 0) throw new NoConstructorsException(String.Format("No constructors {0}", type));

            var maxNumberOfParms = constructors
                .Select(x => x.GetParameters().Length).Max();
            var constructorsWithMaxParams = constructors
                .Where(x => x.GetParameters().Length == maxNumberOfParms).ToList();

            return constructorsWithMaxParams;
        }
    
        private IEnumerable<ConstructorInfo> GetConstructors(Type type)
        {
            List<ConstructorInfo> attributedConstructors = new List<ConstructorInfo>();

            foreach(var constructor in type.GetConstructors())
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





















