using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyInversionEngine
{
    public class DIContainer : ISimpleContainer

    {
        private IInstanceProviderFactory factory = new InstanceProviderFactory();
        private Dictionary<Type, IInstanceProvider> _registeredTypes = new Dictionary<Type, IInstanceProvider>();

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
                _registeredTypes[typeof(T)] = factory.CreateProvider(singleton, typeof(T));
            }
            else
            {
                _registeredTypes.Add(typeof(T), factory.CreateProvider(singleton, typeof(T)));
            }

        }

        public void RegisterType<From, To>(bool singleton) where To : From
        {
            if (_registeredTypes.ContainsKey(typeof(From)))
            {
                _registeredTypes[typeof(From)] = factory.CreateProvider(singleton, typeof(To));
            }
            else
            {
                _registeredTypes.Add(typeof(From), factory.CreateProvider(singleton, typeof(To)));
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
