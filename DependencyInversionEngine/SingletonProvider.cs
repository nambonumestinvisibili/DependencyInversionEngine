using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyInversionEngine
{
    public class SingletonProvider : AbstractInstanceProvider
    {
        private object _instance;

        public SingletonProvider(Type type) : base(type)
        {
         
        }

        public SingletonProvider(object instance)
        {
            _type = instance.GetType();
            _instance = instance;
        }

        public override object Create(Dictionary<Type, IInstanceProvider> registeredTypes)
        {
            if (_instance != null) return _instance;

            _instance = Resolve(registeredTypes);

            return _instance;

        }
    }
}
