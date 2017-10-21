using Microsoft.VisualStudio.TestTools.UnitTesting;
using _2channel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2channel.Tests
{
    [TestClass()]
    public class BoardListTests
    {

        [TestMethod()]
        public void updateTest()
        {
            BoardList bl = new BoardList();
            Board b = bl.findById("news4vip");

            Assert.AreEqual("news4vip", b.id);
            Assert.AreEqual("ニュー速VIP", b.name);
            Assert.AreEqual("hebi.2ch.net", b.host);
            Assert.AreEqual(new Uri("http://hebi.2ch.net/news4vip/"), b.url);
            Assert.AreEqual(new Uri("http://hebi.2ch.net/news4vip/subject.txt"), b.subjectTxt);
            Assert.AreEqual(new Uri("http://hebi.2ch.net/test/bbs.cgi"), b.bbsCgi);
            Assert.AreEqual(new Uri("http://hebi.2ch.net/test/read.cgi/news4vip/"), b.readCgi);
        }

        [TestMethod()]
        public void updateTest1()
        {
            Assert.Fail();
        }
    }
}