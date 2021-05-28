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

            var info = "";

            foreach (var constructorInfo in constructorsInfo)
            {

                try
                {
                    resolvedParams = ResolveConstructor(constructorInfo, registeredTypes);
                }
                catch (Exception e)
                {
                    resolvedParams.Clear();
                    info = " - " + e.Message;
                    continue;
                }

                var instance = constructorInfo.Invoke(resolvedParams.ToArray());
                return instance;
            }

            throw new Exception("Type could not be resolved" + info);
        }

        private List<object> ResolveConstructor(
            ConstructorInfo constructorInfo,
            Dictionary<Type, IInstanceProvider> registeredTypes
            )
        {
            if (CycleDetected(constructorInfo, _type)) {
                throw new Exception("Dependency circuit has been detected");
            }
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
                    throw new Exception("Constructor cannot be resolved - " + e.Message);
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
                throw new Exception(String.Format("Parameter {0} has not been registered", type));
            }
        }

        private bool CycleDetected(ConstructorInfo constructorInfo, Type rootType)
        {
            var parameters = constructorInfo.GetParameters();
            var noneOfTheParametersHaveDependencyOnRootType = 
                parameters.All(x => x.ParameterType != rootType);
            if (!noneOfTheParametersHaveDependencyOnRootType) return true;
            var noDependencyFurther = parameters.All(x =>
            {
                var construcotrs = GetConstructorsWithMaximalNoOfParameters(x.ParameterType);
                var res = true;
                construcotrs.ToList().ForEach(c =>
                {
                    res = res && c.GetParameters().All(z => !CycleDetected(c, rootType));
                });
                return res;
            });
            return !(noneOfTheParametersHaveDependencyOnRootType && noDependencyFurther);
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

        protected IEnumerable<ConstructorInfo> GetConstructorsWithMaximalNoOfParameters(Type type)
        {
            var constructors = new List<ConstructorInfo>(type.GetConstructors());

            var maxNumberOfParms = constructors
                .Select(x => x.GetParameters().Length).Max();
            var constructorsWithMaxParams = constructors
                .Where(x => x.GetParameters().Length == maxNumberOfParms).ToList();

            return constructorsWithMaxParams;
        }
    }
}





















