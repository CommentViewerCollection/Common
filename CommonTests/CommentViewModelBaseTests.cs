using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using SitePlugin;
using NUnit.Framework;
using Common;

namespace CommonTests
{
    [TestFixture]
    public class CommentViewModelBaseTests
    {
        internal class Impl : CommentViewModelBase
        {
            public override string UserId => throw new NotImplementedException();

            public string Do(string text)
            {
                return ExtractNickname(text);
            }
            public Impl(ICommentOptions options) : base(options, new UserTest(""),null, false)
            {

            }
        }
        [Test]
        public void Test()
        {
            var k = new Mock<ICommentOptions>();
            var stk = new Impl(k.Object);
            Assert.AreEqual(null, stk.Do(""));
            Assert.AreEqual(null, stk.Do("@"));
            Assert.AreEqual(null, stk.Do("@11"));
            Assert.AreEqual("a", stk.Do("@a"));
            Assert.AreEqual("a", stk.Do("@a n"));
            Assert.AreEqual("b", stk.Do("@a @b"));
            Assert.AreEqual("a", stk.Do("@11 @a"));
            Assert.AreEqual("a", stk.Do("＠a"));
        }
    }
}
