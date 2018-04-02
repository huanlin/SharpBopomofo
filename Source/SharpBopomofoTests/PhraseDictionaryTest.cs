using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Serilog;
using SharpBopomofo;

namespace SharpBopomofoTests
{
    [TestFixture]
    public class PhraseDictionaryTest
    {
        [OneTimeSetUp]
        public void TestInitialize()
        {
            var logFileName = TestContext.CurrentContext.TestDirectory + "/log.txt";
            if (File.Exists(logFileName))
            {
                File.Delete(logFileName);
            }            

            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(logFileName)
                .MinimumLevel.Debug()
                .CreateLogger();

            TestContext.Out.WriteLine($"測試過程所的 log 會輸出至 {logFileName}。");
        }

        [OneTimeTearDown]
        public void Cleanup()
        {

        }

        private string[] CreateTestPhraseData()
        {
            string[] phrases =
            {
                "一動不如一靜 2 ㄧ ㄉㄨㄥˋ ㄅㄨˋ ㄖㄨˊ ㄧˊ ㄐㄧㄥˋ",
                "一動不如一靜 2 ㄧˊ ㄉㄨㄥˋ ㄅㄨˋ ㄖㄨˊ ㄧ ㄐㄧㄥˋ",
                "一動不如一靜 2 ㄧˊ ㄉㄨㄥˋ ㄅㄨˋ ㄖㄨˊ ㄧˊ ㄐㄧㄥˋ",
                "一去不回頭 0 ㄧˊ ㄑㄩˋ ㄅㄨˋ ㄏㄨㄟˊ ㄊㄡˊ",
                "一字不漏 0 ㄧ ㄗˋ ㄅㄨˊ ㄌㄡˋ",
            };
            return phrases;
        }

        [Test]
        public void Should_AddLaterPhrase_When_DuplicatePhrases_With_SameFrequency()
        {
            // 這組測試資料是用來測試頻率相同的重複片語。
            string[] phrases =
            {
                "一動不如一靜 2 ㄧ ㄉㄨㄥˋ ㄅㄨˋ ㄖㄨˊ ㄧˊ ㄐㄧㄥˋ",
                "一動不如一靜 2 ㄧˊ ㄉㄨㄥˋ ㄅㄨˋ ㄖㄨˊ ㄧ ㄐㄧㄥˋ",
                "一動不如一靜 2 ㄧˊ ㄉㄨㄥˋ ㄅㄨˋ ㄖㄨˊ ㄧˊ ㄐㄧㄥˋ"
            };

            var dictManager = new PhraseDictionaryManager(Log.Logger);
            for (int i = 0; i < phrases.Length; i++)
            {
                dictManager.AddPhrase(phrases[i]);
            }

            var expected = new PhraseData(
                frequency: 2,
                bopomofoList: new List<string>()
                {
                    "ㄧˊ", "ㄉㄨㄥˋ", "ㄅㄨˋ", "ㄖㄨˊ", "ㄧˊ", "ㄐㄧㄥˋ"
                });

            var actual = dictManager.PhraseDictionary["一動不如一靜"];
            Assert.IsTrue(actual.Equals(expected));            
        }

        [Test]
        public void Should_AddHigherFrequencyPhrase_When_DuplicatePhrases_With_DifferentFrequency()
        {
            // 這組測試資料是用來測試頻率不同的重複片語。
            string[] phrases =
            {
                "一動不如一靜 1 ㄧ ㄉㄨㄥˋ ㄅㄨˋ ㄖㄨˊ ㄧˊ ㄐㄧㄥˋ",
                "一動不如一靜 3 ㄧˊ ㄉㄨㄥˋ ㄅㄨˋ ㄖㄨˊ ㄧ ㄐㄧㄥˋ",
                "一動不如一靜 2 ㄧˊ ㄉㄨㄥˋ ㄅㄨˋ ㄖㄨˊ ㄧˊ ㄐㄧㄥˋ"
            };

            var dictManager = new PhraseDictionaryManager(Log.Logger);
            for (int i = 0; i < phrases.Length; i++)
            {
                dictManager.AddPhrase(phrases[i]);
            }

            var expected = new PhraseData(
                frequency: 3,
                bopomofoList: new List<string>()
                {
                    "ㄧˊ", "ㄉㄨㄥˋ", "ㄅㄨˋ", "ㄖㄨˊ", "ㄧ", "ㄐㄧㄥˋ"
                });

            var actual = dictManager.PhraseDictionary["一動不如一靜"];
            Assert.IsTrue(actual.Equals(expected));
        }

        [TestCase("/../../../libchewing/data/tsi.src")]
        public async Task Should_LoadFromFileSucceeded(string filename)
        {
            var dictManager = new PhraseDictionaryManager(Log.Logger);
            await dictManager.LoadFromTextFileAsync(TestContext.CurrentContext.TestDirectory + filename);
        }

        [TestCase("/TestPhrase.bin")]
        public void Should_SerializationSucceeded(string filename)
        {
            var phrases = CreateTestPhraseData();

            var dictManager = new PhraseDictionaryManager(Log.Logger);
            dictManager.LoadFromEnumerable(phrases);

            filename = TestContext.CurrentContext.TestDirectory + filename;
            PhraseDictionarySerializer.SerializeToBinaryFile(filename, dictManager.PhraseDictionary);
            Assert.IsTrue(File.Exists(filename));

            var deserialized = PhraseDictionarySerializer.DeserializeFromBinaryFile(filename);
            Assert.IsTrue(dictManager.PhraseDictionary.Equals(deserialized));
        }

        /// <summary>
        /// 這個測試可以用來產生「處理過的」片語字典檔，以便將來直接以反序列化的方式快速載入字典。產生的檔案是二進位格式。
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task Should_LoadFileAndSerializeToBinarySucceeded()
        {
            string srcFileName = TestContext.CurrentContext.TestDirectory + "/../../../libchewing/data/tsi.src";
            var dictManager = new PhraseDictionaryManager(Log.Logger);
            await dictManager.LoadFromTextFileAsync(srcFileName);

            string dstFileName = TestContext.CurrentContext.TestDirectory + "/ChewingDictionary.bin";
            PhraseDictionarySerializer.SerializeToBinaryFile(dstFileName, dictManager.PhraseDictionary);
            Assert.IsTrue(File.Exists(dstFileName));

            var deserialized = PhraseDictionarySerializer.DeserializeFromBinaryFile(dstFileName);
            Assert.IsTrue(dictManager.PhraseDictionary.Equals(deserialized));
        }

        /// <summary>
        /// 這個測試可以用來產生「處理過的」片語字典檔，以便將來直接以反序列化的方式快速載入字典。產生的檔案是 JSON 格式。
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task Should_LoadFileAndSerializeToJsonSucceeded()
        {
            string srcFileName = TestContext.CurrentContext.TestDirectory + "/../../../libchewing/data/tsi.src";
            var dictManager = new PhraseDictionaryManager(Log.Logger);
            await dictManager.LoadFromTextFileAsync(srcFileName);

            string dstFileName = TestContext.CurrentContext.TestDirectory + "/ChewingDictionary.json";
            PhraseDictionarySerializer.SerializeToJsonFile(dstFileName, dictManager.PhraseDictionary);
            Assert.IsTrue(File.Exists(dstFileName));

            var deserialized = PhraseDictionarySerializer.DeserializeFromJsonFile(dstFileName);
            Assert.IsTrue(dictManager.PhraseDictionary.Equals(deserialized));
        }
    }
}
