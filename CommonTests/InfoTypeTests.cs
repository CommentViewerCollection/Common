using Common;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTests
{
    [TestFixture]
    public class InfoTypeTests
    {
        [Test]
        public void OrderingTest()
        {
            Assert.IsTrue(InfoType.Debug > InfoType.Notice);
            Assert.IsTrue(InfoType.Notice > InfoType.Error);
            Assert.IsTrue(InfoType.Error > InfoType.None);
        }
        [Test]
        public void InfoTypeFromStringTest()
        {
            Assert.AreEqual(InfoType.Debug, InfoTypeRelatedOperations.ToInfoType("Debug"));
            Assert.AreEqual(InfoType.Notice, InfoTypeRelatedOperations.ToInfoType("Notice"));
            Assert.AreEqual(InfoType.None, InfoTypeRelatedOperations.ToInfoType("None"));
            Assert.AreEqual(InfoType.Notice, InfoTypeRelatedOperations.ToInfoType("a"));
        }
    }
}
