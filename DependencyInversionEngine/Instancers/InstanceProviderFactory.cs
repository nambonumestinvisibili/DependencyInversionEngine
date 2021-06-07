using DependencyInversionEngine.InstanceProviders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyInversionEngine
{
    public interface IInstancerFactory
    {
        public IInstanceProvider CreateProvider(bool isSingleton, Type type, Dictionary<Type, IInstanceProvider> registeredTypes);
        public IInstanceProvider CreateProvider(object instance);
        public IInstanceBuilder CreateBuilder(object instance, Dictionary<Type, IInstanceProvider> registeredTypes);
    }

    internal class InstanceProviderFactory : IInstancerFactory
    {
        public IInstanceBuilder CreateBuilder(object instance, Dictionary<Type, IInstanceProvider> registeredTypes)
        {
            return new InstanceBuilder(instance, registeredTypes);
        }

        public IInstanceProvider CreateProvider(bool isSingleton, Type type, Dictionary<Type, IInstanceProvider> registeredTypes)
        {
            if (isSingleton)
            {
                return new SingletonProvider(type);
            }
            else
            {
                return new TransientProvider(type, registeredTypes);
            }
        }

        public IInstanceProvider CreateProvider(object instance)
        {
            return new SingletonProvider(instance);
        }
    }
}
