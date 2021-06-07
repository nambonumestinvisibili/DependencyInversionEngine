using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyInversionEngine.ServiceLocator
{
    public delegate DIContainer ContainerProviderDelegate();
    public class ServiceLocator
    {
        private static ContainerProviderDelegate _containerProvider;
        
        public static void SetContainerProvider(ContainerProviderDelegate containerProvider)
        {
            _containerProvider = containerProvider;
        }

        public static DIContainer Current
        {
            get
            {
                if (_containerProvider == null)
                    throw new ContainerProviderNotSetException();
                return _containerProvider();
            }

        }

        public static void SetAlwaysTheSameContainer()
        {
            DIContainer dIContainer = new DIContainer();
            ContainerProviderDelegate del = () => dIContainer;
            SetContainerProvider(del);
        }

        public static void SetAlwaysDifferentContainer()
        {
            ContainerProviderDelegate del = () => new DIContainer();
            SetContainerProvider(del);
        }

   }
}
