using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace _2channel
{
    public class ThreadList
    {
        private Board board;
        private string subject;
        private Dictionary<long, Thre> preDict = new Dictionary<long, Thre>();
        private Dictionary<long, Thre> nowDict = new Dictionary<long, Thre>();
        private List<Thre> deletedThreadsList = new List<Thre>();

        /// <summary>
        /// 板のURLで初期化すること
        /// </summary>
        /// <param name="boardUrl">ex. https://hebi.5ch.net/news4vip/ </param>
        public ThreadList(Board board)
        {
            this.subject = "";
            this.board = board;
        }

        public void update()
        {
            if(getSubject_txt())
            {
                createThreadList();
            }
        }

        #region getSubject.txt
        /// <summary>
        /// subject.textを元にしてスレ一覧の情報を求める
        /// 例外終了で白紙のsubject.textを保持
        /// </summary>
        /// <returns>正常終了/例外終了</returns>
        private bool getSubject_txt()
        {
            var wc = new System.Net.WebClient();

            try /*正常に読み込めたらファイルにそれを書き込む　*/
            {
                this.subject = wc.DownloadString(board.subjectTxt);
            }
            catch (Exception)
            {
                //読込エラーがでたら白紙で保持する
                this.subject = "";
                return false;
            }
            finally
            {
                wc.Dispose();
            }
            //正常なファイルが読み込めるまで読み込みを繰り返す
            if (isValidFile() == false)
            {
                getSubject_txt();
            }
            return true;
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
            using (var sr = new StringReader(this.subject))
            {
                //最後の行を読みこむ
                while (sr.Peek() != -1)
                {
                    line = sr.ReadLine();
                }
            }

            int ERROR_LENGTH = 5;
            //1行が5文字以下の場合、正常なファイルでない可能性が高いし以下の処理でエラーが出る
            //総レス数を示す(1000)の部分のあれ
            if (line.Length < ERROR_LENGTH)
                return false;

            //末尾5文字を抜き出して数字が1～4桁含まれていれば正常なファイルと判断する
            line = line.Substring(line.Length - ERROR_LENGTH);
            Regex regex = new Regex(@"\(\d{1,4}?\)");
            return regex.IsMatch(line);
        }
        #endregion

        private void createThreadList()
        {
            nowDict = null; //初期化
            nowDict = new Dictionary<long, Thre>();

            bool isFirst = (this.preDict == null);   //この処理を行うのは初めてかどうか
            using (var sr = new StringReader(this.subject))
            {
                for(int depth = 1; sr.Peek() != -1; depth++)
                {
                    string line = sr.ReadLine();
                    Thre thre = new Thre(this.board, line);
                    if (isFirst)
                        this.preDict.Add(thre.key, thre);
                    else
                        this.nowDict.Add(thre.key, thre);
                }
            }
            //初めての処理なら以前との比較ができないのでここでreturn
            if (isFirst)
                return;

            //落ちたスレのリストを更新
            searchDeletedThread();

            //現在生存しているスレについて情報の更新
            updateThreadInfomation();

            preDict.Clear();
            preDict = nowDict;
        }

        /// <summary>
        /// 前回取得したスレ一覧と今回取得したスレ一覧を比較し、落ちたスレを求める
        /// </summary>
        private void searchDeletedThread()
        {
            this.deletedThreadsList.Clear();
            foreach (KeyValuePair<long, Thre> pre in preDict)
            {
                if (nowDict.ContainsKey(pre.Key) == false)
                {
                    deletedThreadsList.Add(pre.Value);
                }
            }
        }

        /// <summary>
        /// 前回取得したスレ一覧と今回取得したスレ一覧を比較し、総レス数などの更新を行う
        /// </summary>
        private void updateThreadInfomation()
        {
            DateTime dtnow = DateTime.Now;
            foreach (KeyValuePair<long, Thre> now in nowDict)
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
        }

        /// <summary>
        /// 引数で指定した語句を含む/含まないスレのListを返す
        /// </summary>
        /// <param name="match">～を含む</param>
        /// <param name="unmatch">～を含まない</param>
        /// <returns></returns>
        public List<Thre> findByWord(List<string> match, List<string> unmatch = null)
        {
            this.update();

            List<Thre> ret = new List<Thre>();

            //スレタイに引数を含むスレのリストを取得
            foreach(Thre thre in nowDict.Values)
            {
                bool isAllMatch = true;
                foreach (string word in match)
                {
                    //大文字小文字の違いを無視　一致すれば0以上の整数を返す
                    if (thre.title.IndexOf(word, StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        isAllMatch = false;
                        break;
                    }
                }
                // 引数のワードを全て含んだスレのみを返却する
                if(isAllMatch)
                    ret.Add(thre);
            }
            //不一致ワードの指定がないならここでreturn
            if (unmatch == null)
                return ret;

            //上記リストから不一致ワードを含むリストを作成
            var unnecessary = new List<Thre>();
            foreach(Thre thre in ret)
            {
                foreach(string word in unmatch)
                {
                    // 不一致語句をどれか一つでも含むと除外とする
                    if (thre.title.IndexOf(word, StringComparison.OrdinalIgnoreCase) > 0)
                        unnecessary.Add(thre);
                }
                
            }
            //不一致ワードを含むものをリストから削除
            foreach(Thre thre in unnecessary)
            {
                ret.Remove(thre);
            }
            return ret;
        }

        public List<Thre> findByKey(int threadKey)
        {
            this.update();

            List<Thre> ret = new List<Thre>();

            foreach (KeyValuePair<long, Thre> now in nowDict)
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
