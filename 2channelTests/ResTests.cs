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
    public class ResTests
    {
        [TestMethod()]
        public void getPrecisionWriteTimeTest()
        {
            BoardList bl = new BoardList();
            Board b = bl.findById("news4vip");
            Thre thre = new Thre(b, "1507545723.dat<>出来るプログラミング言語を書いていって30挙げられた奴から初心者終了 	 (4)");
            var li = thre.getResList();
            string a = li.First().writeTime;
            //string aaa = a.ToShortTimeString();

        }
    }
}