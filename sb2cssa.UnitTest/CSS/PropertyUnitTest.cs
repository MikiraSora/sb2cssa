using Microsoft.VisualStudio.TestTools.UnitTesting;
using sb2cssa.CSS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sb2cssa.UnitTest.CSS
{
    [TestClass]
    [TestCategory(nameof(PropertyUnitTest))]
    public class PropertyUnitTest
    {
        [TestMethod]
        public void NameValueTest()
        {
            Property a = new Property("a");
            a.Value = new StringValue("b");

            Assert.AreEqual("a", a.Name);
            Assert.AreEqual("b", a.Value.FormatAsCSSSupport(null));
        }

        [TestMethod]
        public void EqualTest()
        {
            Property a = new Property("a", "b");
            Property b = new Property("a", "b");

            var x = a.Equals(a, b);

            Assert.IsTrue(x);
        }
    }
}
