using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyInversionEngine
{
    public interface IDependencyInversionContainer
    {
        public void RegisterType<T>(bool Singleton) where T : class;
        public void RegisterType<From, To>(bool Singleton) where To : From;
        public void RegisterInstance<T>(T instance);
        public T Resolve<T>() where T : class;
        public void BuildUp<T>(T instance);
    }
}
