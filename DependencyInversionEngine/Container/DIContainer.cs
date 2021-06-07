using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyInversionEngine
{
    public class DIContainer : IDependencyInversionContainer

    {
        private IInstanceProviderFactory factory = new InstanceProviderFactory();
        private Dictionary<Type, IInstanceProvider> _registeredTypes = new Dictionary<Type, IInstanceProvider>();

        public void BuildUp<T>(T instance)
        {
            IInstanceBuilder instanceBuilder = factory.CreateBuilder(instance, _registeredTypes);
            instanceBuilder.BuildInstance();
        }

        public void RegisterInstance<T>(T instance)
        {
            if (_registeredTypes.ContainsKey(typeof(T)))
            {
                _registeredTypes[typeof(T)] = factory.CreateProvider(instance);
            }
            else
            {
                _registeredTypes.Add(typeof(T), factory.CreateProvider(instance));
            }
        }

        public void RegisterType<T>(bool singleton) where T : class
        {
           

            if (_registeredTypes.ContainsKey(typeof(T)))
            {
                _registeredTypes[typeof(T)] = factory.CreateProvider(singleton, typeof(T), _registeredTypes);
            }
            else
            {
                _registeredTypes.Add(typeof(T), factory.CreateProvider(singleton, typeof(T), _registeredTypes));
            }

        }

        public void RegisterType<From, To>(bool singleton) where To : From
        {
            if (_registeredTypes.ContainsKey(typeof(From)))
            {
                _registeredTypes[typeof(From)] = factory.CreateProvider(singleton, typeof(To), _registeredTypes);
            }
            else
            {
                _registeredTypes.Add(typeof(From), factory.CreateProvider(singleton, typeof(To), _registeredTypes));
            }
        }

        public T Resolve<T>() where T : class
        {
            if (_registeredTypes.ContainsKey(typeof(T)))
            {
                return (T)_registeredTypes[typeof(T)].Create(_registeredTypes);
            }
            else if (typeof(T).IsInterface)
            {
                throw new Exception("Interface implementation has not been registered");
            }
            else
            {
                RegisterType<T>(false);
                return (T)_registeredTypes[typeof(T)].Create(_registeredTypes);
            }
        }



        
    }
}
