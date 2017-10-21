using System;
using System.IO;
using System.Text;
using System.Net;
using System.Web;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace _2channel
{
    public class UseCgis
    {
        private Encoding enc;
        private CookieContainer cc;
        private string roninSessionID;
        

        public UseCgis()
        {
            this.enc = Encoding.GetEncoding("shift_jis");
            this.cc = new CookieContainer();
            this.roninSessionID = "";
        }

        private string postRequest(Board board, string postData)
        {
            byte[] postDataBytes = Encoding.ASCII.GetBytes(postData);

            var req = (HttpWebRequest)WebRequest.Create(board.bbsCgi);

            req.Method = "POST";
            req.Host = board.host;
            req.Referer = board.url.ToString();
            req.UserAgent = "Monazilla/2.00 (JaneStyle/3.83)";  // 浪人ログイン時のセッションＩＤ先頭参照
            req.ContentType = "application/x-www-form-urlencoded";
            req.KeepAlive = false;
            req.Accept = "*/*";
            req.CookieContainer = this.cc;
            req.ContentLength = postDataBytes.Length;

            string response = "";
            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(postDataBytes, 0, postDataBytes.Length);
                WebResponse res = req.GetResponse();
                using (var resStream = res.GetResponseStream())
                {
                    using (var sr = new StreamReader(resStream, this.enc))
                    {
                        response = sr.ReadToEnd();
                    }
                }
                this.cc = req.CookieContainer;
            }

            return response;
        }

        public bool kakiko(Board board, string key, string body, string mail="", string name="", bool isUseRonin=false)
        {
            //投稿するクエリを作成
            var param = new Dictionary<string, string>();
            param.Add("bbs", board.id);
            param.Add("time", "1");
            param.Add("key", key);
            param.Add("MESSAGE", body);
            param.Add("mail", mail);
            param.Add("FROM", name);
            param.Add("submit", "書き込む");
            param.Add("yuki", "akari");
            if (isUseRonin)
                param.Add("sid", this.roninSessionID);

            string query = makeQueryString(param, this.enc);

            // リクエストの生成
            string response = postRequest(board, query);
            if (response.Contains("<title>■ 書き込み確認 ■</title>"))
            {
                // cookieが未セットだった場合にこの処理に入る
                // cookieがセットされたはずなので再送する
                response = postRequest(board, query);
            }
            if (response.Contains("<title>書きこみました。</title>"))
                return true;

            return false;
        }

        private string makeQueryString(IDictionary<string, string> dict, Encoding enc)
        {
            var sb = new StringBuilder();
            foreach(var item in dict)
            {
                sb.Append(HttpUtility.UrlEncode(item.Key, enc));
                sb.Append("=");
                sb.Append(HttpUtility.UrlEncode(item.Value, enc));
                sb.Append("&");
            }
            sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
        }

        /// <summary>
        /// 浪人にログインする
        /// </summary>
        /// <params name="mail">浪人登録時のメールアドレス(ID)</params>
        /// <params name="pass">パスワード</params>
        public void roninLogin(string mail, string pass)
        {
            Uri baseUrl = new Uri("https://2chv.tora3.net/");
            Uri authUrl = new Uri(baseUrl, "futen.cgi");

            var wc = new WebClient();
            wc.Encoding = this.enc;

            wc.Headers[HttpRequestHeader.ContentType] =  "application/x-www-form-urlencoded";
            wc.Headers[HttpRequestHeader.Host] = baseUrl.Host;
            wc.Headers[HttpRequestHeader.Referer] = baseUrl.OriginalString;
            wc.Headers[HttpRequestHeader.UserAgent] = "DOLIB/1.00";
            wc.Headers[HttpRequestHeader.AcceptLanguage] = "ja";
            wc.Headers[HttpRequestHeader.KeepAlive] = "false";
            wc.Headers.Add("X-2ch-UA", "JaneStyle/3.83");

            NameValueCollection nvc = HttpUtility.ParseQueryString(string.Empty, this.enc);
            nvc.Add("ID", mail);
            nvc.Add("PW", pass);
            wc.QueryString = nvc;

            //レスポンス(セッションID)を取得
            byte[] result = wc.DownloadData(authUrl);
            string response = this.enc.GetString(result);

            //レスポンスの整形
            response = response.Substring(11);
            response = response.Replace("\n", "");
            response = response.Replace("\r\n", "");

            //正常に取得できなかった場合
            if (roninSessionID.IndexOf("ERROR:pppp") >= 0)
            {
                this.roninSessionID = "";
            }
            else
            {
                this.roninSessionID = response;
            }
        }

        #region 浪人ログアウト
        /// <summary>
        /// 浪人をログアウトする
        /// </summary>
        public void roninLogout()
        {
            this.roninSessionID = "";
        }
        #endregion
    }
}
