using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyInversionEngine
{
    [AttributeUsage(AttributeTargets.Constructor)]
    public class DependencyConstructor : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class DependencyProperty : Attribute
    {

    }
}
