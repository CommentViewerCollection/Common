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
    class LoggerTestTests
    {
        [Serializable]
        class ParseException : Exception
        {
            public string Raw { get; }
            public ParseException(string raw)
            {
                Raw = raw;
            }
            public ParseException(string raw, Exception inner) : base("", inner)
            {
                Raw = raw;
            }
        }
        [Test]
        public void 例外のプロパティの値が記録されているか()
        {
            var logger = new LoggerTest();
            try
            {
                throw new ParseException("abc");
            }
            catch (ParseException ex)
            {
                logger.LogException(ex);
            }
            Assert.IsTrue(logger.GetExceptions().Contains("\"Raw\":\"abc\""));
        }
    }
}
