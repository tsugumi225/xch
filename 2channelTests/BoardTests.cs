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
    public class BoardTests
    {
        [TestMethod()]
        public void BoardTest()
        {
            Board b = new Board(new Uri("https://hebi.2ch.net/news4vip/"), "ニュー速VIP");
            Assert.AreEqual("news4vip", b.id);
            Assert.AreEqual("ニュー速VIP", b.name);
            Assert.AreEqual("hebi.2ch.net", b.host);
            Assert.AreEqual(new Uri("https://hebi.2ch.net/news4vip/"), b.url);
            Assert.AreEqual(new Uri("https://hebi.2ch.net/news4vip/subject.txt"), b.subjectTxt);
            Assert.AreEqual(new Uri("https://hebi.2ch.net/test/bbs.cgi"), b.bbsCgi);
            Assert.AreEqual(new Uri("https://hebi.2ch.net/test/read.cgi/news4vip/"), b.readCgi);
        }
    }
}