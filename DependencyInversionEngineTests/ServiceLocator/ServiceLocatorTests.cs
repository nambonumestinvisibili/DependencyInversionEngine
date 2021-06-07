using Microsoft.VisualStudio.TestTools.UnitTesting;
using DependencyInversionEngine.ServiceLocator;
using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyInversionEngine.ServiceLocator.Tests
{
    [TestClass()]
    public class ServiceLocatorTests
    {
        

        [TestMethod()]
        public void SetAlwaysTheSameContainerTest()
        {
            ServiceLocator.SetAlwaysTheSameContainer();
            var d1 = ServiceLocator.Current;
            var d2 = ServiceLocator.Current;
            Assert.ReferenceEquals(d1, d2);
        }

        [TestMethod()]
        public void SetAlwaysDifferentContainerTest()
        {
            ServiceLocator.SetAlwaysDifferentContainer();
            var d1 = ServiceLocator.Current;
            var d2 = ServiceLocator.Current;
            Assert.IsFalse(ReferenceEquals(d1, d2));
        }

        [TestMethod()]
        public void ResolveTest() {
            ServiceLocator.SetAlwaysDifferentContainer();
            Assert.IsTrue(ServiceLocator.Current.Resolve<A>() != null);
        }

        [TestMethod()]
        public void ResolveTest2()
        {
            Foo foo = new Foo();
            Assert.IsTrue(foo.Bar() != null);
        }

        public class A
        {
            
        }

        class Foo
        {
            public A Bar()
            {
                ServiceLocator.SetAlwaysTheSameContainer();
                A service = ServiceLocator.Current.Resolve<A>();
                return service;
            }
        }
    }
}