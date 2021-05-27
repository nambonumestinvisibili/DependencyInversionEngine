using Microsoft.VisualStudio.TestTools.UnitTesting;
using DependencyInversionEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyInversionEngine.Tests
{
    [TestClass()]
    public class DIContainerTests
    {
        
        [TestMethod()]
        public void ShouldReceivedDifferentObjects()
        {
            ISimpleContainer simpleContainer = new DIContainer();
            simpleContainer.RegisterType<Foo>(false);

            var f = simpleContainer.Resolve<Foo>();
            var f2 = simpleContainer.Resolve<Foo>();

            Assert.IsFalse(Assert.ReferenceEquals(f, f2));

        }

        [TestMethod()]
        public void ShouldReceivedTheSameObject()
        {
            ISimpleContainer simpleContainer = new DIContainer();
            simpleContainer.RegisterType<Foo>(true);

            var f = simpleContainer.Resolve<Foo>();
            var f2 = simpleContainer.Resolve<Foo>();

            Assert.ReferenceEquals(f, f2);

        }

        [TestMethod()]
        public void ShouldThrowExceptionWhenInterfaceImplementationNotRegistered()
        {
            ISimpleContainer simpleContainer = new DIContainer();


            Assert.ThrowsException<Exception>(() =>
            {
                var f = simpleContainer.Resolve<IFoo>();
            });
        }

        [TestMethod()]
        public void ShouldNOTThrowExceptionWhenTypeNotRegistered() //Uwaga 1
        {
            ISimpleContainer simpleContainer = new DIContainer();

            var f = simpleContainer.Resolve<Foo>();
            Assert.IsTrue(f.GetType() == typeof(Foo));
        }

        [TestMethod()]
        public void ShouldReturnRegisteredImplementationOfInterface()
        {
            ISimpleContainer simpleContainer = new DIContainer();

            simpleContainer.RegisterType<IFoo, Foo>(true);
            var impl = simpleContainer.Resolve<IFoo>();

            bool result = impl.GetType() == typeof(Foo); 
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void ShouldReturnLatestRegisteredImplementationOfInterface()
        {
            ISimpleContainer simpleContainer = new DIContainer();

            simpleContainer.RegisterType<IFoo, Foo>(true);
            var impl = simpleContainer.Resolve<IFoo>();

            simpleContainer.RegisterType<IFoo, Foo2>(true);
            var impl2 = simpleContainer.Resolve<IFoo>();

            bool result = impl.GetType() != impl2.GetType();
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void ShouldReturnTheSameObject()
        {
            ISimpleContainer simpleContainer = new DIContainer();

            simpleContainer.RegisterType<IFoo, Foo>(true);
            var impl = simpleContainer.Resolve<IFoo>();

            var impl2 = simpleContainer.Resolve<IFoo>();

            Assert.ReferenceEquals(impl, impl2);
        }

        [TestMethod()]
        public void ShouldReturnDifferentObjects()
        {
            ISimpleContainer simpleContainer = new DIContainer();

            simpleContainer.RegisterType<IFoo, Foo>(false);
            var impl = simpleContainer.Resolve<IFoo>();

            var impl2 = simpleContainer.Resolve<IFoo>();

            Assert.IsFalse(Assert.ReferenceEquals(impl, impl2));
        }

        //Tests TaskSet DI 2
        
        [TestMethod()]
        public void ShouldRegisterInstance()
        {
            ISimpleContainer simpleContainer = new DIContainer();

            IFoo f1 = new Foo();
            simpleContainer.RegisterInstance<IFoo>(f1);

            IFoo f2 = simpleContainer.Resolve<IFoo>();

            Assert.IsTrue(ReferenceEquals(f1, f2));
        }

        [TestMethod()]
        public void ShouldCreateTypeWithParams_SingletonV()
        {
            ISimpleContainer simpleContainer = new DIContainer();

            simpleContainer.RegisterType<B>(true);
            simpleContainer.RegisterType<A>(true);
            A a = simpleContainer.Resolve<A>();

            Assert.IsTrue(a.b != null);
        }

        [TestMethod()]
        public void ShouldCreateTypeWithParams_TransientV()
        {
            ISimpleContainer simpleContainer = new DIContainer();

            simpleContainer.RegisterType<B>(false);
            simpleContainer.RegisterType<A>(false);
            A a = simpleContainer.Resolve<A>();

            Assert.IsTrue(a.b != null);
        }

        [TestMethod()]
        public void ShouldThrowExceptionWhenTryingToConstructInstanceWIthUnregisteredParameter()
        {
            ISimpleContainer simpleContainer = new DIContainer();

            simpleContainer.RegisterType<A>(false);


            Assert.ThrowsException<Exception>(() =>
            {
                A a = simpleContainer.Resolve<A>();
            });
        }
    }

    public interface IFoo
    {

    }

    public class Foo : IFoo
    {

    }

    public class Foo2 : IFoo
    {

    }

    public class A
    {
        public B b;

        public A( B b )
        {
            this.b = b;
        }
    }

    public class B { }
}