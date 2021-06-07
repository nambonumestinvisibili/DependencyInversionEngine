using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using DependencyInversionEngine.ConstructorAnalyzer;
using DependencyInversionEngine.InstanceProviders;

namespace DependencyInversionEngine
{
    public interface IInstanceProvider
    {
        public object Create(Dictionary<Type, IInstanceProvider> registeredTypes);
    }

    internal abstract class AbstractInstanceProvider : IInstanceProvider
    {
        public abstract object Create(Dictionary<Type, IInstanceProvider> registeredTypes);

        protected Type _type;
        protected DependenciesResolver resolver;

        public AbstractInstanceProvider(Type type)
        {
            _type = type;
            resolver = new DependenciesResolver(_type);
        }
        public AbstractInstanceProvider()
        {

        }

    }
}





















