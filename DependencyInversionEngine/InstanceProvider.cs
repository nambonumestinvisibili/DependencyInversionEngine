using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
