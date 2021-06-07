using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyInversionEngine.InstanceProviders
{
    internal class InstanceBuilder : IInstanceBuilder
    {
        private object instance;
        private DependenciesResolver resolver;
        private Dictionary<Type, IInstanceProvider> registeredTypes;

        public InstanceBuilder(object instance, Dictionary<Type, IInstanceProvider> registeredTypes)
        {
            this.instance = instance;
            resolver = new DependenciesResolver(instance.GetType());
            this.registeredTypes = registeredTypes;
        }

        public void BuildInstance()
        {
            resolver.InjectProperties(instance, registeredTypes);
        }
    }
}
