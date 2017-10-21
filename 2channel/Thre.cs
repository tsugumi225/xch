using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace _2channel
{
    public class Thre
    {
        private Board board;
        public long key { get; set; }
        public string title { get; set; }
        public int resCount { get; set; }
        public DateTime FirstWriteTime { get; set; }
        public DateTime LastWriteTime { get; set; }
        public TimeSpan lifeTime { get; set; }
        public TimeSpan elapsedTime { get; set; }

        private List<Res> resList = new List<Res>();
        public List<Res> newArrivalRes = new List<Res>();

        /// <summary>
        /// 初期化処理 lineにはsubject.txtから読み取った一行を入れること
        /// </summary>
        /// <param name="board"></param>
        /// <param name="line">ex. 1507478543.dat<>VIPでスプラトゥーン 	 (55)</param>
        public Thre(Board board, string line)
        {
            this.board = board;
            Regex re = new Regex(@"(?<key>\d{10})\.dat\<\>(?<title>.*)\t*.*\((?<resCount>\d{1,4})\)$");
            Match match = re.Match(line);
            this.key = long.Parse(match.Groups["key"].Value);
            this.title = match.Groups["title"].Value;
            this.resCount = int.Parse(match.Groups["resCount"].Value);
        }

        /// <summary>
        /// ThreadListから呼び出される
        /// 最終書き込みと生存時間を更新する
        /// </summary>
        /// <param name="dtnow"></param>
        /// <param name="before"></param>
        internal void update(DateTime dtnow, Thre before)
        {
            //レスが増えてた
            if (this.resCount > before.resCount)
            {
                this.LastWriteTime = dtnow; //最終書き込み時間を更新する
            }
            else
            {
                this.LastWriteTime = before.LastWriteTime;  //最終書き込み時間を引きつぐ
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
                downloadString = wc.DownloadString(new Uri(board.readCgi, key + "/"));
            }

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
