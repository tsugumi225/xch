using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp;

namespace _2channel
{
    public class ThreadInfomation
    {
        public long threadKey { get; set; }
        public string threadTitle { get; set; }
        public int resCount { get; set; }
        public int depth { get; set; }
        public DateTime FirstWriteTime { get; set; }
        public DateTime LastWriteTime { get; set; }
        public TimeSpan lifeTime { get; set; }
        public TimeSpan elapsedTime { get; set; }

        private List<Res> resList = new List<Res>();
        public List<Res> newArrivalRes = new List<Res>();

        public ThreadInfomation(int depth , string line)
        {

            this.depth = depth;
            Regex re = new Regex(@"(?<threadKey>\d{10})\.dat\<\>(?<title>.*)\t*.*\((?<resCount>\d{1,4})\)$");
            Match match = re.Match(line);
            this.threadKey = long.Parse(match.Groups["threadKey"].Value);
            this.threadTitle = match.Groups["title"].Value;
            this.resCount = int.Parse(match.Groups["resCount"].Value);
        }

        /// <summary>
        /// ThreadListから呼び出される
        /// 最終書き込みと生存時間を更新する
        /// </summary>
        /// <param name="dtnow"></param>
        /// <param name="before"></param>
        internal void update(DateTime dtnow, ThreadInfomation before)
        {
            //レスが増えてた
            if(this.resCount > before.resCount)
            {
                //最終書き込み時間を更新する
                this.LastWriteTime = dtnow;
            }
            else
            {
                //最終書き込み時間を引きつぐ
                this.LastWriteTime = before.LastWriteTime;
            }
            //立った時刻を引き継ぐ
            this.FirstWriteTime = before.FirstWriteTime;
            //最終書き込みからの経過時間算出
            this.elapsedTime = dtnow - this.LastWriteTime;
            //生存時間算出しなおし
            this.lifeTime = dtnow - this.FirstWriteTime;
            //レス一覧引き継ぎ
            this.resList = before.resList;
        }

        /// <summary>
        /// 再取得。新着レスも取得できる
        /// </summary>
        /// <returns>新着レス群</returns>
        public void updateResList()
        {
            List<Res> newArrival = new List<Res>();

            string downloadString = "";
            using (WebClient wc = new WebClient())
            {
                downloadString = wc.DownloadString("http://hebi.2ch.net/test/read.cgi/news4vip/" + threadKey + "/");
            }
            //using (StringReader sr = new StringReader(downloadString))
            //{
            //int cnt = 0;
            //while (true)
            //{
            //    string line = sr.ReadLine();
            //    //最終行を過ぎたらbreak
            //    if (line == null) break;

            //    //4文字以下は確実に違う
            //    if (line.Length <= 4) continue;
            //    //dtを含まないものは違う
            //    if (line.Contains("<dt>") == false) continue;

            //    if (line.Contains("<dt>") == true && line.Contains("<div ") == true)
            //    {
            //        //>>2とかの広告表示対策
            //        Regex reg = new Regex(@"<div id.*<\/div>");
            //        line = reg.Replace(line, "");
            //    }
            //    cnt++;
            //    if (cnt > resList.Count)
            //    {
            //        newArrival.Add(new Res(line));
            //    }
            //}
            //resList.AddRange(newArrival);
            //this.newArrivalRes = newArrival;
            //}

            var parser = new AngleSharp.Parser.Html.HtmlParser();
            var document = parser.Parse(downloadString);
            var postedList = document.GetElementsByClassName("post");
            for(int i = this.resList.Count; i < postedList.Count(); i++)
            {
                newArrival.Add(new Res(postedList[i]));
            }
            this.resList.AddRange(newArrival);
            this.newArrivalRes = newArrival;
        }

        /// <summary>
        /// すべての書き込みを取得する
        /// </summary>
        /// <returns></returns>
        public List<Res> getResList()
        {
            updateResList();
            return this.resList;
        }

        /// <summary>
        /// 指定した数の最新レスを取り出す
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<Res> getResList(int count)
        {
            updateResList();
            return this.resList.Reverse<Res>().Take(count).Reverse<Res>().ToList();
        }

        public DateTime getPrecisionLastWriteTime()
        {
            //一番最後のレスを取得
            Res res = this.resList[resList.Count - 1];
            return res.getPrecisionWriteTime();
        }
    }
}
