using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;

namespace _2channel
{
    /// <summary>
    /// 板一覧を取得
    /// 最初にgetリクエストが走るからちょっと時間かかるので注意
    /// </summary>
    public class BoardList
    {
        List<Board> boardlist;

        public BoardList()
        {
            boardlist = new List<Board>();
            update();
        }

        /// <summary>
        /// 板一覧を取得し、板名と板URLの対応を保持する
        /// </summary>
        public void update()
        {
            boardlist.Clear();
            var url = "http://menu.2ch.net/bbsmenu.html";
            using (WebClient wc = new WebClient())
            {
                var downloadString = wc.DownloadString(url);
                var parser = new AngleSharp.Parser.Html.HtmlParser();
                var document = parser.Parse(downloadString);
                var aa = document.QuerySelectorAll("A");
                foreach(AngleSharp.Dom.Html.IHtmlAnchorElement item in aa)
                {
                    // 板のURLでないと思われるリンクは除外
                    if (!item.Href.Contains("ch.net/"))
                        continue;
                    var board = new Board(new Uri(item.Href), item.InnerHtml);
                    //板をリストに追加する
                    //重複がある場合はスキップ
                    if (boardlist.Contains(board))
                        continue;

                    boardlist.Add(board);
                }
            }
        }

        /// <summary>
        /// 板名から板の情報を取得する
        /// </summary>
        /// <param name="boardName">ex. ニュー速VIP </param>
        /// <returns>板の情報</returns>
        public Board findByName(string boardName)
        {
            return boardlist.Where(x => x.name == boardName).ToList().First();
        }

        /// <summary>
        /// 板のIDから板の情報を取得する
        /// </summary>
        /// <param name="boardId">ex. news4vip </param>
        /// <returns>板の情報</returns>
        public Board findById(string boardId)
        {
            return boardlist.Where(x => x.id == boardId).ToList().First();
        }
    }
}
