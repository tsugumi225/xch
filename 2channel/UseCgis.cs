using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace _2channel
{
    public class UseCgis
    {
        private string roninSessionID = "";
        private CookieCollection cc = new CookieCollection();

        private string postRequest(string postData)
        {
            //バイト型配列に変換
            byte[] postDataBytes = System.Text.Encoding.ASCII.GetBytes(postData);

            //WebRequestの作成
            System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create("http://hebi.2ch.net/test/bbs.cgi");
            //クッキーのなんか
            req.CookieContainer = new CookieContainer();
            req.CookieContainer.Add(this.cc);
            //ContentTypeを"application/x-www-form-urlencoded"にする
            req.ContentType = "application/x-www-form-urlencoded";
            //メソッドにPOSTを指定
            req.Method = "POST";
            //ホストに板のある鯖を設定
            req.Host = "hebi.2ch.net";
            //POST送信するデータの長さを指定
            req.ContentLength = postDataBytes.Length;
            //リファラーに板のURLを設定
            req.Referer = "http://hebi.2ch.net/news4vip/";
            //
            req.UserAgent = "Monazilla/2.00 (JaneStyle/3.83)";  /*浪人ログイン時のセッションＩＤ先頭参照*/
            //
            req.KeepAlive = false;
            req.Accept = "*/*";

            //データをPOST送信するためのStreamを取得
            Stream reqStream = req.GetRequestStream();
            //送信するデータを書き込む
            reqStream.Write(postDataBytes, 0, postDataBytes.Length);
            reqStream.Close();

            //サーバーからの応答を受信するためのWebResponseを取得
            WebResponse res = req.GetResponse();
            //応答データを受信するためのStreamを取得
            Stream resStream = res.GetResponseStream();
            //受信して隠し項目を作る←やっぱなし
            StreamReader sr = new StreamReader(resStream, Encoding.GetEncoding("shift_jis"));
            string memo = sr.ReadToEnd();
            resStream.Close();
            sr.Close();

            this.cc = req.CookieContainer.GetCookies(req.RequestUri);

            return memo;
        }

        private string createPostData(string key, string body, string mail, string name,string button, bool isUseRonin)
        {
            //文字コードを指定する
            System.Text.Encoding enc = System.Text.Encoding.GetEncoding("shift_jis");
            //スレキー
            string threadKey = key;
            //書き込み本文
            string mainfield = System.Web.HttpUtility.UrlEncode(body, enc);
            //メール欄
            string mailfield = System.Web.HttpUtility.UrlEncode(mail);
            //名前欄
            string namefield = System.Web.HttpUtility.UrlEncode(name);
            //書き込むボタン
            string writebutton = System.Web.HttpUtility.UrlEncode(button);

            string postData = "bbs=news4vip"
                            + "&time=1"
                            + "&key=" + threadKey
                            + "&MESSAGE=" + mainfield
                            + "&mail=" + mailfield
                            + "&FROM=" + namefield
                            + "&submit=" + writebutton
                            + "&yuki = akari";
            if (isUseRonin)
            {
                string sessionID = System.Web.HttpUtility.UrlEncode(this.roninSessionID, enc);
                postData += "&sid=" + sessionID;
            }
            return postData;
        }

        public bool executeWriting(string key, string body, string mail, string name, bool isUseRonin)
        {

            //POST送信する文字列を作成
            string postData = createPostData(key, body, mail, name, "書き込む", isUseRonin);
            string returnBody = postRequest(postData);

            if (returnBody.Contains("<title>■ 書き込み確認 ■</title>"))
            {
                postData = createPostData(key, body, mail, name, "上記全てを承諾して書き込む", isUseRonin);
                postRequest(postData);
                return true;
            }
            else if(returnBody.Contains("<title>書きこみました。</title>"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void roninLogin(string mail, string pass)
        {
            string mailaddress = mail;
            string password = pass;

            //POST送信する文字列を作成
            string postData = "ID=" + mailaddress + "&PW=" + password;
            //バイト型配列に変換
            byte[] postDataBytes = System.Text.Encoding.ASCII.GetBytes(postData);
            //WebRequestの作成
            System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create("https://2chv.tora3.net/futen.cgi?guid=ON HTTP/1.1");
            req.ContentType = "application/x-www-form-urlencoded";
            req.Method = "POST";
            //ホストに板のある鯖を設定
            req.Host = "2chv.tora3.net";
            req.Referer = "https://2chv.tora3.net/";
            req.UserAgent = "DOLIB/1.00";
            req.Headers.Add("X-2ch-UA", "JaneStyle/3.83");
            req.Headers.Add("Accept-Language", "ja");
            //POST送信するデータの長さを指定
            req.ContentLength = postDataBytes.Length;
            req.KeepAlive = false;

            //データをPOST送信するためのStreamを取得
            System.IO.Stream reqStream = req.GetRequestStream();
            //データ送信
            reqStream.Write(postDataBytes, 0, postDataBytes.Length);
            reqStream.Close();

            //サーバーからの応答を受信するためのWebResponseを取得
            System.Net.WebResponse res = req.GetResponse();
            //応答データを受信するためのStreamを取得
            System.IO.Stream resStream = res.GetResponseStream();

            byte[] rdData;
            byte[] buf = new byte[32768]; // 一時バッファ

            using (MemoryStream ms = new MemoryStream())
            {
                while (true)
                {
                    // ストリームから一時バッファに読み込む
                    int read = resStream.Read(buf, 0, buf.Length);

                    if (read > 0)
                    {
                        // 一時バッファの内容をメモリ・ストリームに書き込む
                        ms.Write(buf, 0, read);
                    }
                    else
                    {
                        break;
                    }
                }
                // メモリ・ストリームの内容をバイト配列に格納
                rdData = ms.ToArray();
            }

            string roninSessionID = System.Text.Encoding.GetEncoding("shift_jis").GetString(rdData);
            resStream.Close();

            ////加工前のセッションＩＤ書き出してみる
            //System.IO.StreamWriter writer = new System.IO.StreamWriter(@"ronin.txt");
            //writer.Write(roninSessionID);
            //writer.Close();

            roninSessionID = roninSessionID.Substring(11);
            roninSessionID = roninSessionID.Replace("\n", "");
            roninSessionID = roninSessionID.Replace("\r\n", "");

            if (roninSessionID.IndexOf("ERROR:pppp") >= 0)
            {
                this.roninSessionID = "";
            }
            else
            {
                this.roninSessionID = roninSessionID;
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
