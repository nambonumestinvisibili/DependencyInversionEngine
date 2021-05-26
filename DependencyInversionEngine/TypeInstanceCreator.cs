using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyInversionEngine
{
    public abstract class TypeInstanceCreator
    {

        public bool _isSingleton {get; private set;}
        private Type _type;
        private Func<object> _typeInstanceCreator;
        private object _singletonInstance;
        public TypeInstanceCreator(bool isSingleton, Type type) 
        {
            _type = type;
            _isSingleton = isSingleton;
            _typeInstanceCreator = () => Activator.CreateInstance(type);
            _singletonInstance = Activator.CreateInstance(type);
            
        }

        public TypeInstanceCreator(object instance)
        {
            _type = instance.GetType();
            _isSingleton = true;
            _typeInstanceCreator = () => Activator.CreateInstance(_type);
            _singletonInstance = instance;
        }

        public object Create(Dictionary<Type, TypeInstanceCreator> registeredTypes)
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
