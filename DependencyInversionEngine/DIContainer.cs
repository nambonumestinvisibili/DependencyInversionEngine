using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyInversionEngine
{
    public class DIContainer : ISimpleContainer
    {
        private Dictionary<Type, Func<object>> _transientDictionary = new Dictionary<Type, Func<object>>();
        private Dictionary<Type, object> _singletonDictionary = new Dictionary<Type, object>();
        private Dictionary<Type, TypeInstanceCreator> _registeredTypes = new Dictionary<Type, DependencyInversionEngine.TypeInstanceCreator>();

        public void RegisterType<T>(bool singleton) where T : class
        {

            if (_registeredTypes.ContainsKey(typeof(T)))
            {
                _registeredTypes[typeof(T)] = new TypeInstanceCreator(singleton, typeof(T));
            }
            else
            {
                _registeredTypes.Add(typeof(T), new TypeInstanceCreator(singleton, typeof(T)));
            }

        }

        public void RegisterType<From, To>(bool singleton) where To : From
        {
            if (_registeredTypes.ContainsKey(typeof(From)))
            {
                _registeredTypes[typeof(From)] = new TypeInstanceCreator(singleton, typeof(To));
            }
            else
            {
                _registeredTypes.Add(typeof(From), new TypeInstanceCreator(singleton, typeof(To)));
            }
        }

        public T Resolve<T>() where T : class
        {
            if (_registeredTypes.ContainsKey(typeof(T)))
            {
                return (T)_registeredTypes[typeof(T)].Create();
            }
            else if (typeof(T).IsInterface)
            {
                throw new Exception("Interface implementation has not been registered");
            }
            else
            {
                RegisterType<T>(false);
                return (T)_registeredTypes[typeof(T)].Create();
            }
        }

        
    }
}
