using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyInversionEngine
{
    public class TransientProvider : AbstractInstanceProvider
    {

        private Func<object> _provider;
        public TransientProvider(Type type) : base(type)
        {
            _provider = () => Activator.CreateInstance(type);
        }

        public override object Create(Dictionary<Type, IInstanceProvider> registeredTypes)
        {
            return _provider.Invoke();
        }
    }
}
