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
    public class ThreadInfomationTests
    {
        [TestMethod()]
        public void getResListTest()
        {
            //ThreadInfomation ti = new ThreadInfomation(369, "1475937735.dat<>【狩りゲー】VIPでテイルズウィーバーの要塞制圧【TW】 [無断転載禁止]&#169;2ch.net		(204)");
            //ti.getResList();
        }

        [TestMethod()]
        public void updateResListTest()
        {
            var ti = new ThreadInfomation(1, "1502626156.dat<>【大型アプデ】VIPでテイルズウィーバー制圧するぞ【TW】 [無断転載禁止]&#169;2ch.net	 (496)");
            ti.updateResList();
            DateTime dt = ti.getResList().First().getPrecisionWriteTime();
            Assert.Fail();
        }
    }
}