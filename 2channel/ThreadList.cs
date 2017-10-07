using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace _2channel
{
    public class ThreadList
    {
        private string subject;
        private Dictionary<long, ThreadInfomation> preDict = new Dictionary<long, ThreadInfomation>();
        private Dictionary<long, ThreadInfomation> nowDict = new Dictionary<long, ThreadInfomation>();
        private List<ThreadInfomation> deletedThreadsList = new List<ThreadInfomation>();

        public ThreadList()
        {
            this.subject = "";
        }

        #region getSubject.txt
        /// <summary>
        /// subject.textを元にしてスレ一覧の情報を求める
        /// 例外終了で白紙のsubject.textを保存
        /// </summary>
        /// <returns>正常終了/例外終了</returns>
        public bool getSubject_txt()
        {
            System.Net.WebClient wc = new System.Net.WebClient();

            bool ret = false;

            try /*正常に読み込めたらファイルにそれを書き込む　*/
            {
                this.subject = wc.DownloadString("http://hebi.2ch.net/news4vip/subject.txt");
                ret = true;
            }
            catch (Exception)　/*　読込エラーがでたらなにもしない　*/
            {
                return false;
            }
            finally　/*閉じる*/
            {
                wc.Dispose();
            }
            if (isValidFile() == false)
            {
                getSubject_txt();
            }
            createThreadList();
            return ret;
        }
        #endregion

        #region isValidFile
        /// <summary>
        /// subject.textがちゃんとしたファイルか
        /// </summary>
        /// <returns>正常ファイル/異常ファイル</returns>
        private bool isValidFile()
        {
            string line = "";
            using (System.IO.StringReader sr = new System.IO.StringReader(this.subject))
            {
                //最後の行を読みこむ
                while (sr.Peek() != -1)
                {
                    line = sr.ReadLine();
                }
            }
            if (line.Length >= 6) /* 6なことに意味はあまりない　バグ回避*/
            {
                //末尾6文字を抜き出して数字が1～4桁含まれていれば正常なファイルと判断する
                line = line.Substring(line.Length - 6);
                Regex regex = new Regex(@"\(\d{1,4}?\)");

                return regex.IsMatch(line);
            }
            else
            {
                return false;
            }
        }
        #endregion

        private void createThreadList()
        {
            //一番最初
            if(preDict == null)
            {
                using (System.IO.StringReader sr = new System.IO.StringReader(this.subject))
                {
                    int cnt = 0;
                    while (sr.Peek() != -1)
                    {
                        cnt++;
                        string line = sr.ReadLine();
                        ThreadInfomation ti = new ThreadInfomation(cnt, line);
                        preDict.Add(ti.threadKey, ti);
                    }
                }
                //念のためコピー
                nowDict = preDict;
                return;
            }

            //二回目以降以下
            nowDict = null;
            nowDict = new Dictionary<long, ThreadInfomation>();
            DateTime dtnow = DateTime.Now;

            using (System.IO.StringReader sr = new System.IO.StringReader(this.subject))
            {
                int cnt = 0;
                while (sr.Peek() != -1)
                {
                    cnt++;
                    string line = sr.ReadLine();
                    ThreadInfomation ti = new ThreadInfomation(cnt, line);
                    nowDict.Add(ti.threadKey, ti);
                }
            }

            //落ちたスレを求める
            this.deletedThreadsList.Clear();
            foreach (KeyValuePair<long, ThreadInfomation> pre in preDict)
            {
                if (nowDict.ContainsKey(pre.Key) == false)
                {
                    deletedThreadsList.Add(pre.Value);
                }
            }

            //現在生存しているスレについて
            foreach (KeyValuePair<long, ThreadInfomation> now in nowDict)
            {
                //以前からあったスレの場合
                if (preDict.ContainsKey(now.Key))
                {
                    now.Value.update(dtnow, preDict[now.Key]);
                }
                //新しくたったスレの場合
                else
                {
                    now.Value.FirstWriteTime = dtnow;
                    now.Value.LastWriteTime = dtnow;
                }
            }
            preDict.Clear();
            preDict = nowDict;
        }


        /// <summary>
        /// 引数で指定した語句を含むスレのListを返す
        /// </summary>
        /// <param name="match">～を含む</param>
        /// <returns></returns>
        public List<ThreadInfomation> searchThreadOnMatch(string match)
        {
            getSubject_txt();

            List<ThreadInfomation> ret = new List<ThreadInfomation>();

            foreach(KeyValuePair<long, ThreadInfomation> now in nowDict)
            {
                //大文字小文字の違いを無視　一致すれば0以上の整数を返す
                if (now.Value.threadTitle.IndexOf(match,StringComparison.OrdinalIgnoreCase) != -1)
                {
                    ret.Add(now.Value);
                }
            }
            return ret;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="match">一致する語句</param>
        /// <param name="list">探すスレのリスト</param>
        /// <returns></returns>
        public List<ThreadInfomation> searchThreadOnMatch(string match, List<ThreadInfomation> list)
        {
            List<ThreadInfomation> ret = new List<ThreadInfomation>();

            foreach (ThreadInfomation ti in list)
            {
                //大文字小文字の違いを無視　一致すれば0以上の整数を返す
                if (ti.threadTitle.IndexOf(match, StringComparison.OrdinalIgnoreCase) != -1)
                {
                    ret.Add(ti);
                }
            }
            return ret;
        }

        /// <summary>
        /// 引数で指定した語句を含まないスレのListを返す
        /// </summary>
        /// <param name="match">～を含む</param>
        /// <param name="misMatch">～を含まない</param>
        /// <returns></returns>
        public List<ThreadInfomation> searchThreadOnMismatch(string misMatch)
        {
            List<ThreadInfomation> ret = new List<ThreadInfomation>();

            foreach (KeyValuePair<long, ThreadInfomation> now in nowDict)
            {
                //大文字小文字の違いを無視　一致すれば0以上の整数を返す
                if (now.Value.threadTitle.IndexOf(misMatch, StringComparison.OrdinalIgnoreCase) == -1)
                {
                    ret.Add(now.Value);
                }
            }
            return ret;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="misMatch">一致しない語句</param>
        /// <param name="list">探すスレのリスト</param>
        /// <returns></returns>
        public List<ThreadInfomation> searchThreadOnMismatch(string misMatch, List<ThreadInfomation> list)
        {
            List<ThreadInfomation> ret = new List<ThreadInfomation>();

            foreach (ThreadInfomation ti in list)
            {
                //大文字小文字の違いを無視　一致すれば0以上の整数を返す
                if (ti.threadTitle.IndexOf(misMatch, StringComparison.OrdinalIgnoreCase) == -1)
                {
                    ret.Add(ti);
                }
            }
            return ret;
        }

        public List<ThreadInfomation> searchThread(int threadKey)
        {
            getSubject_txt();

            List<ThreadInfomation> ret = new List<ThreadInfomation>();

            foreach (KeyValuePair<long, ThreadInfomation> now in nowDict)
            {
                if (now.Key == threadKey)
                {
                    ret.Add(now.Value);
                }
            }
            return ret;
        }
    }
}
