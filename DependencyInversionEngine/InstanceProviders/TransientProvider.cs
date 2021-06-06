using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyInversionEngine
{
    internal class TransientProvider : AbstractInstanceProvider
    {

        private Func<object> _provider;
        public TransientProvider(Type type, Dictionary<Type, IInstanceProvider> registeredTypes) : base(type)
        {
            _provider = () => Resolve(registeredTypes);
        }

        public override object Create(Dictionary<Type, IInstanceProvider> registeredTypes)
        {
            return _provider.Invoke();
        }
    }
}
