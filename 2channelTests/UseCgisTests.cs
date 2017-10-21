using _2channel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace _2channel.Tests
{
    [TestClass()]
    public class UseCgisTests
    {

        [TestMethod()]
        public void kakikoTest()
        {
            var aaa = new UseCgis();
            aaa.roninLogin("", "");
            var bl = new BoardList();
            var b = bl.findById("news4vip");
            aaa.kakiko(b, "1508422307", "てすと", "", "", true);
        }

        //[TestMethod()]
        //public void makeQueryStringTest()
        //{
        //    var aaa = new Dictionary<string,string>();
        //    aaa.Add("a", "あ");
        //    aaa.Add("i", "い");
        //    aaa.Add("u", "う");

        //    var cgi = new UseCgis();
        //    string result = cgi.makeQueryString(aaa, Encoding.UTF8);

        //}
    }
}