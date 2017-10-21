using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2channel
{
    public class Board
    {
        public string id { get; }
        public string name { get; }
        public string host { get; }
        public Uri url { get; }       
        public Uri subjectTxt { get; }
        public Uri bbsCgi { get; }
        public Uri readCgi { get; }

        /// <summary>
        /// 初期化処理
        /// </summary>
        /// <param name="url">ex. https://hebi.5ch.net/news4vip/ </param>
        /// <param name="name">ex. ニュー速VIP </param>
        public Board(Uri url, string name)
        {
            this.id = url.AbsolutePath.Replace("/", "");    //news4vip部分を取得
            this.name = name;
            this.host = url.Host;

            this.url = url;
            this.subjectTxt = new Uri(url, "subject.txt");
            this.bbsCgi = new Uri(url.GetLeftPart(UriPartial.Authority) + "/test/bbs.cgi");
            this.readCgi = new Uri(url.GetLeftPart(UriPartial.Authority) + "/test/read.cgi/" + id + "/");
        }
    }
}
