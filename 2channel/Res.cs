using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace _2channel
{
    public class Res
    {
        //public string rowData { get; set; }

        public int resNumber { get; set; }
        public string name { get; set; }
        public string mail { get; set; }
        public string  writeTime { get; set; }
        public string id { get; set; }
        public string body { get; set; }


        public Res(AngleSharp.Dom.IElement element)
        {
            this.resNumber = int.Parse(element.GetElementsByClassName("number")[0].TextContent);
            this.name = element.GetElementsByClassName("name")[0].TextContent;
            //ブラウザから見るとメール欄の情報ガナイ
            //this.mail = element.GetElementsByClassName("mail")[0].TextContent;
            this.mail = "";
            this.writeTime = element.GetElementsByClassName("date")[0].TextContent;
            this.id = element.GetElementsByClassName("uid")[0].TextContent.Substring(3);
            this.body = element.GetElementsByClassName("escaped")[0].TextContent;

        }

        public DateTime getPrecisionWriteTime()
        {
            DateTime dt = DateTime.ParseExact(this.writeTime,"yyyy/MM/dd(ddd) HH:mm:ss.fff", null);
            return dt;
        }
    }
}
