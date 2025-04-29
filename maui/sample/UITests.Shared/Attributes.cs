using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UITests.Shared
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class WrittenBy : Attribute
    {
        public string Name { get; set; }

        public WrittenBy(string name)
        {
            Name = name;
        }
    }
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class ValidatedBy : Attribute
    {
        public string Name { get; set; }

        public Platform Platform { get; set; }

        public ValidatedBy(string name, Platform platform)
        {
            Name = name;
            Platform = platform;
        }
    }
    public enum Platform
    {
        Both = 0,
        iOS = 2,
        Android = 4,
    }
}
