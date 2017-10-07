using Microsoft.VisualStudio.TestTools.UnitTesting;
using _2channel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2channel.Tests
{
    [TestClass()]
    public class ResTests
    {
        [TestMethod()]
        public void ResTest()
        {
            string line = @"<dt>31 ：<a href=""mailto:sage""><b>以下、無断転載禁止でVIPがお送りします</b></a>：2016/10/08(土) 23:50:40.112 ID:ws9Y9aEVd<dd> ワキガデブとか最悪やんけw <br><br>";
            Res res = new Res(line);
            Assert.AreEqual(31, res.resNumber);
            Assert.AreEqual("以下、無断転載禁止でVIPがお送りします", res.name);
            Assert.AreEqual("23:50:40", res.writeTime);
            Assert.AreEqual("ws9Y9aEVd", res.id);
            Assert.AreEqual("ワキガデブとか最悪やんけw", res.body);
            Assert.AreEqual("sage", res.mail);
            DateTime dt = res.getPrecisionWriteTime();

            line = "<dt>4 ：<font color=green><b>以下、無断転載禁止でVIPがお送りします</b></font>：2016/10/08(土) 23:43:13.537 ID:OmEZ6/lm0<dd> のりこめー <br><br>";
            res = new Res(line);
            Assert.AreEqual(4, res.resNumber);
            Assert.AreEqual("以下、無断転載禁止でVIPがお送りします", res.name);
            Assert.AreEqual("23:43:13", res.writeTime);
            Assert.AreEqual("OmEZ6/lm0", res.id);
            Assert.AreEqual("のりこめー", res.body);
            Assert.AreEqual("", res.mail);
            dt = res.getPrecisionWriteTime();

            line = @"<div id=""js--banners--thread"" class=""banner"" style=""width: 100 %; ""></div><dt>2 ：<font color=green><b>以下、無断転載禁止でVIPがお送りします</b></font>：2016/10/08(土) 23:42:27.116 ID:0iYqpRKu0<dd> キャラに迷うやつ向け簡易キャラ需要解説表 <br> <br> ティチエル　：唯一のヒーラーかつ必須バフ3種持ち　 　 　 　 <br> イサック.....　：対人最強アタッカー　やり込むほどに強くなる <br> マキシミン....：最高の命中率を誇り多芸に通ずる魔法剣士 <br> ジョシュア.....：いないと負けるエンチャンター　 <br> --------------------大活躍キャラの壁-------------------- <br> アナイス. 　 ：聖域を展開して攻守両面から味方を守る <br> ボリス..　　　：初心者からプロまで幅広く活躍する自走式人間爆弾 <br> イソレット......：デバッファー　対イサック兵器 <br> ミラ....　　　　：主に後衛での妨害役 <br> ナヤトレイ....：主に最前線での妨害役 <br> ---------------------活躍キャラの壁--------------------- <br> ノクターン.....：射程と手数と攻撃速度に優れる <br> クロエ..　　　：高めの機動力を活かした偵察等 <br> ------------------------ 高い壁 ------------------------ <br> ルシ･ピン･シベ･ランジ･ロアミニ･ベンヤ：作る価値なし <br> <br> ※ノクターンとロアミニは初期作成不可 <br><";
            res = new Res(line);
            Assert.AreEqual(2, res.resNumber);
            Assert.AreEqual("以下、無断転載禁止でVIPがお送りします", res.name);
            Assert.AreEqual("23:42:27", res.writeTime);
            Assert.AreEqual("0iYqpRKu0", res.id);
            //Assert.AreEqual("のりこめー", res.body);

        }
    }
}