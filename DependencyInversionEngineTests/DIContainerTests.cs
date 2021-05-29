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

        [TestMethod()]
        public void ShouldUseSecondConstructor()
        {
            ISimpleContainer simpleContainer = new DIContainer();

            simpleContainer.RegisterType<D>(false);
            simpleContainer.RegisterType<C>(false);
            C c = simpleContainer.Resolve<C>();

            Assert.IsTrue(c.d != null);
        }

        [TestMethod()]
        public void ShouldDetectCycle()
        {
            ISimpleContainer simpleContainer = new DIContainer();

            simpleContainer.RegisterType<E>(false);
            simpleContainer.RegisterType<C>(false);

            Assert.ThrowsException<DependencyCycleException>(() =>
           {
               C c = simpleContainer.Resolve<C>();

           });
        }

        [TestMethod()]
        public void ShouldDetectCycle2()
        {
            ISimpleContainer simpleContainer = new DIContainer();

            simpleContainer.RegisterType<F>(false);
            simpleContainer.RegisterType<C>(false);

            Assert.ThrowsException<DependencyCycleException>(() =>
            {
                C c = simpleContainer.Resolve<C>();
                Console.WriteLine();
            });
        }

        [TestMethod()]
        public void ShouldPickAttributedConstrucotrFirst()
        {
            ISimpleContainer simpleContainer = new DIContainer();

            simpleContainer.RegisterType<B>(false);
            simpleContainer.RegisterType<D>(false);
            simpleContainer.RegisterType<G>(false);

            var res = simpleContainer.Resolve<G>();

            Assert.IsTrue(res.d != null);
        }

        [TestMethod()]
        public void ShouldThrowExceptionWhenTheOnlyConstructorHasParametersThatCanNotBeResolved()
        {
            ISimpleContainer simpleContainer = new DIContainer();

            simpleContainer.RegisterType<Y>(false);
            simpleContainer.RegisterType<X>(false);

            Assert.ThrowsException<NoConstructorsException>(() =>
            {
                var res = simpleContainer.Resolve<X>();
            });

        }

        [TestMethod()]
        public void ShouldCreateTypeWhenTheOnlyConstructorHasParametersThatCanNotBeResolvedButIsRegisteredAsInstance()
        {
            ISimpleContainer simpleContainer = new DIContainer();

            string s = "ala ma kota";
            simpleContainer.RegisterInstance<string>(s);
            simpleContainer.RegisterType<Y>(false);
            simpleContainer.RegisterType<X>(false);

            var res = simpleContainer.Resolve<X>();

            Assert.IsTrue(res.s == s);
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

    public class C {
        public B b;
        public D d;
        public E e;
        public F f;

        public C (B b)
        {
            this.b = b;
        }

        public C (D d)
        {
            this.d = d;
        }

        public C(E e)
        {
            this.e = e;
        }

        public C(F f)
        {
            this.f = f;
        }
    
    }

    public class D { }
    public class E
    {
        public C c;
        public E(C c)
        {
            this.c = c;
        }
    }
    public class F
    {
        public E e;
        public F(E e)
        {
            this.e = e;
        }
    }

    public class G
    {
        public B b;
        public D d;

        public G(B b)
        {
            this.b = b;
        }

        [DependencyConstructor]
        public G(D d)
        {
            this.d = d;
        }
    }

    public class X
    {
        Y d;
        public string s;
        public X(Y d, string s) {
            this.d = d;
            this.s = s;
        }
    }

    public class Y { }

}