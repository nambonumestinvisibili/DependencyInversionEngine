using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyInversionEngine
{
    public class TypeInstanceCreator
    {

        public bool _isSingleton {get; private set;}
        private Type _type;
        private Func<object> _typeInstanceCreator;
        private static object _singletonInstance;
        public TypeInstanceCreator(bool isSingleton, Type type) 
        {
            _type = type;
            _isSingleton = isSingleton;
            _typeInstanceCreator = () => Activator.CreateInstance(type);
            _singletonInstance = Activator.CreateInstance(type);
            
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
