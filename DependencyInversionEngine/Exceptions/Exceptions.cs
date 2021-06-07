using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyInversionEngine
{
    public class ConstructorUnresolvableException : Exception
    {
        public ConstructorUnresolvableException(string message) : base(message)
        {
        }
    }

    public class DependencyCycleException : Exception
    {
        public DependencyCycleException(string message) : base(message)
        {
        }
    }

    public class NoConstructorsException : Exception
    {
        public NoConstructorsException(string message) : base(message)
        {
        }
    }

    public class UnregisteredParameterException : Exception
    {
        public UnregisteredParameterException(string message) : base(message)
        {
        }
    }

    public class PropertyUnresolvableException : Exception
    {
        public PropertyUnresolvableException(string message) : base(message)
        {
        }
    }

    public class NotPublicPropertyInjectionException : Exception
    {
        public NotPublicPropertyInjectionException(string message) : base(message)
        {
        }
    }

    public class ContainerProviderNotSetException : Exception
    {
        public ContainerProviderNotSetException()
        {
        }
    }
}
