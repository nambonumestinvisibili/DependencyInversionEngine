using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyInversionEngine
{
    public class DIContainer : ISimpleContainer
    {
        private Dictionary<Type, Func<object>> _transientDictionary = new Dictionary<Type, Func<object>>();
        private Dictionary<Type, object> _singletonDictionary = new Dictionary<Type, object>();

        public void RegisterType<T>(bool singleton) where T : class
        {
            if (singleton)
            {
                if (!_singletonDictionary.ContainsKey(typeof(T)))
                {
                    _singletonDictionary.Add(typeof(T), Activator.CreateInstance<T>());
                }
            }
            else
            {
                if (!_transientDictionary.ContainsKey(typeof(T)))
                {
                    _transientDictionary.Add(typeof(T), () => Activator.CreateInstance<T>());
                }
            }
            RemoveFromTheOtherDictionary<T>(singleton);
        }

        public void RegisterType<From, To>(bool singleton) where To : From
        {
            if (singleton)
            {
                if (!_singletonDictionary.ContainsKey(typeof(From)))
                {
                    _singletonDictionary.Add(typeof(From), Activator.CreateInstance<To>());


                }
                else
                {
                    if (_singletonDictionary[typeof(From)].GetType() != typeof(To))
                    {
                        _singletonDictionary[typeof(From)] = Activator.CreateInstance<To>();
                    }
                }
                
            }
            else
            {
                if (!_transientDictionary.ContainsKey(typeof(From)))
                {
                    _transientDictionary.Add(typeof(From), () => Activator.CreateInstance<To>());
                }
                else
                {
                    if (_transientDictionary[typeof(From)].GetType() != typeof(To))
                    {
                        _transientDictionary[typeof(From)] = () => Activator.CreateInstance<To>();
                    }
                }
                
            }

            RemoveFromTheOtherDictionary<From>(singleton);
        }

        public T Resolve<T>() where T : class
        {
            if (_singletonDictionary.ContainsKey(typeof(T)))
            {
                return (T)_singletonDictionary[typeof(T)];
            }
            else if (_transientDictionary.ContainsKey(typeof(T)))
            {
                return (T)_transientDictionary[typeof(T)].Invoke();
            }
            else if (typeof(T).IsInterface)
            {
                throw new Exception("Interface implementation has not been registered");
            }
            else
            {
                RegisterType<T>(false);
                return (T)_transientDictionary[typeof(T)].Invoke();

            }
        }

        private void RemoveFromTheOtherDictionary<T>(bool singleton)
        {
            if (singleton)
            {
                if (_transientDictionary.ContainsKey(typeof(T)))
                {
                    _transientDictionary.Remove(typeof(T));
                }
            }
            else
            {
                if (_singletonDictionary.ContainsKey(typeof(T)))
                {
                    _singletonDictionary.Remove(typeof(T));

                }
            }
        }
    }
}
