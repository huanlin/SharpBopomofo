using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using SharpBopomofo;

namespace SharpBopomofoTests
{
    [TestClass]
    public class PhraseTableTest
    {
        [TestInitialize]
        public void TestInitialize()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Debug()
                .CreateLogger();
        }

        [TestMethod]
        public void Should_AddLaterPhrase_When_ThereAreDuplicatePhrases()
        {
            string[] lines =
            {
                "一動不如一靜 2 ㄧ ㄉㄨㄥˋ ㄅㄨˋ ㄖㄨˊ ㄧˊ ㄐㄧㄥˋ",
                "一動不如一靜 2 ㄧˊ ㄉㄨㄥˋ ㄅㄨˋ ㄖㄨˊ ㄧ ㄐㄧㄥˋ",
                "一動不如一靜 2 ㄧˊ ㄉㄨㄥˋ ㄅㄨˋ ㄖㄨˊ ㄧˊ ㄐㄧㄥˋ"
            };

            var table = new PhraseTable(Log.Logger);
            for (int i = 0; i < 3; i++)
            {
                table.AddPhrase(lines[i]);
            }

            var expected = new PhraseData()
            {
                Frequency = 2,
                BopomofoList = new List<string>()
                {
                    "ㄧˊ", "ㄉㄨㄥˋ", "ㄅㄨˋ", "ㄖㄨˊ", "ㄧˊ", "ㄐㄧㄥˋ"
                }
            };

            var actual = table["一動不如一靜"];
            Assert.IsTrue(actual.Equals(expected));            
        }

        [TestMethod]
        public void Should_AddHigherFrequencyPhrase_When_ThereAreDuplicatePhrases()
        {
            string[] lines =
            {
                "一動不如一靜 1 ㄧ ㄉㄨㄥˋ ㄅㄨˋ ㄖㄨˊ ㄧˊ ㄐㄧㄥˋ",
                "一動不如一靜 3 ㄧˊ ㄉㄨㄥˋ ㄅㄨˋ ㄖㄨˊ ㄧ ㄐㄧㄥˋ",
                "一動不如一靜 2 ㄧˊ ㄉㄨㄥˋ ㄅㄨˋ ㄖㄨˊ ㄧˊ ㄐㄧㄥˋ"
            };

            var table = new PhraseTable(Log.Logger);
            for (int i = 0; i < 3; i++)
            {
                table.AddPhrase(lines[i]);
            }

            var expected = new PhraseData()
            {
                Frequency = 3,
                BopomofoList = new List<string>()
                {
                    "ㄧˊ", "ㄉㄨㄥˋ", "ㄅㄨˋ", "ㄖㄨˊ", "ㄧ", "ㄐㄧㄥˋ"
                }
            };

            var actual = table["一動不如一靜"];
            Assert.IsTrue(actual.Equals(expected));
        }

    }
}
