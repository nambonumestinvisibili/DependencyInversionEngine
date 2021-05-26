using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyInversionEngine
{
    public interface IInstanceProviderFactory
    {
        public IInstanceProvider CreateProvider(bool isSingleton, Type type);
        public IInstanceProvider CreateProvider(object instance);
    }

    public class InstanceProviderFactory : IInstanceProviderFactory
    {

        public IInstanceProvider CreateProvider(bool isSingleton, Type type)
        {
            if (isSingleton)
            {
                return new SingletonProvider(type);
            }
            else
            {
                return new TransientProvider(type);
            }
        }

        public IInstanceProvider CreateProvider(object instance)
        {
            return new SingletonProvider(instance);
        }
    }
}
