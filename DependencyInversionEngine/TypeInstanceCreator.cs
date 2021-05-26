using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyInversionEngine
{
    public class TypeInstanceCreator<T> where T : class
    {

        public bool _isSingleton {get; private set;}
        private T _type;
        private Func<T> _typeInstanceCreator;
        private static object _singletonInstance;
        public TypeInstanceCreator(bool isSingleton) 
        {
            _isSingleton = isSingleton;
            _typeInstanceCreator = () => Activator.CreateInstance<T>();
            _singletonInstance = Activator.CreateInstance<T>();
            
        }

        public object Create()
        {
            if (_isSingleton)
            {
                return _singletonInstance;
            }
            else
            {
                return _typeInstanceCreator.Invoke();
            }
        }


    }
}
