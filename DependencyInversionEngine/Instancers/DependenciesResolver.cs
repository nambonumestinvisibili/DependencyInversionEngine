using DependencyInversionEngine.ConstructorAnalyzer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DependencyInversionEngine.InstanceProviders
{
    public class DependenciesResolver
    {
        protected Type _type;
        private readonly ConstructorAnalisator constructorAnalisator = new ConstructorAnalisator();

        public DependenciesResolver(Type type)
        {
            _type = type;
        }

        public object Resolve(Dictionary<Type, IInstanceProvider> registeredTypes)
        {
            var resolvedParams = new List<object>();

            var constructorsInfo = constructorAnalisator.GetConstructors(_type);

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
                InjectProperties(instance, registeredTypes);
                return instance;
            }

            throw new ConstructorUnresolvableException("Type could not be resolved" + info);
        }
        public void InjectProperties(object instance, Dictionary<Type, IInstanceProvider> registeredTypes)
        {
            var propertiesWithAttributeInfo = _type
                .GetProperties()
                .Where(property => Attribute.IsDefined(property, typeof(DependencyProperty)));

            foreach (var propInfo in propertiesWithAttributeInfo)
            {
                if (propInfo.GetSetMethod().IsPublic)
                {
                    ResolveProperty(propInfo, registeredTypes, instance); //it injects...

                }
                else
                {
                    throw new NotPublicPropertyInjectionException(propInfo.GetType().ToString() + " is not");
                }
            }

        }
        private object ResolveProperty(PropertyInfo propertyInfo, Dictionary<Type, IInstanceProvider> registeredTypes, object owner)
        {
            Type propType = propertyInfo.PropertyType;
            var constructors = constructorAnalisator.GetConstructorsWithMaximalNoOfParameters(propType);

            var resolvedParams = new List<object>();
            var info = " ";

            foreach (var constructorInfo in constructors)
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

                propertyInfo.SetValue(owner, instance);
                return instance;
            }

            throw new PropertyUnresolvableException("Property could not be resolved" + info);

        }
        private List<object> ResolveConstructor(
            ConstructorInfo constructorInfo,
            Dictionary<Type, IInstanceProvider> registeredTypes
            )
        {
            if (CycleDetected(constructorInfo, _type, registeredTypes))
            {
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
                var construcotrs = constructorAnalisator.GetConstructorsWithMaximalNoOfParameters(x.ParameterType);
                var res = true;
                construcotrs.ToList().ForEach(c =>
                {
                    res = res && c.GetParameters().All(z => !CycleDetected(c, rootType, registeredTypes));
                });
                return res;
            });
            return !(noneOfTheParametersHaveDependencyOnRootType && noDependencyFurther);
        }
    }
}
