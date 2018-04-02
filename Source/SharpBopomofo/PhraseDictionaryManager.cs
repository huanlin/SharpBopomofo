using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace SharpBopomofo
{
    /// <summary>
    /// 此類別可將 libchewing 的 tsc.src 檔案載入至字典結構（載入過程會丟棄重複的片語），以及把字典結構序列化成 protobuf 格式的二進位檔案。
    /// 
    /// 每個片語可能有多種注音字根，例如：
    /// 
    /// 一動不如一靜 2 ㄧ ㄉㄨㄥˋ ㄅㄨˋ ㄖㄨˊ ㄧ ㄐㄧㄥˋ
    /// 一動不如一靜 2 ㄧ ㄉㄨㄥˋ ㄅㄨˋ ㄖㄨˊ ㄧˊ ㄐㄧㄥˋ
    /// 一動不如一靜 2 ㄧˊ ㄉㄨㄥˋ ㄅㄨˋ ㄖㄨˊ ㄧ ㄐㄧㄥˋ
    /// 一動不如一靜 2 ㄧˊ ㄉㄨㄥˋ ㄅㄨˋ ㄖㄨˊ ㄧˊ ㄐㄧㄥˋ
    /// 
    /// 其中的 2 是出現頻率，數字越大代表越常出現，優先權越高。
    /// </summary>    
    public class PhraseDictionaryManager
    {
        public PhraseBopomofoDictionary PhraseDictionary { get; private set; } = new PhraseBopomofoDictionary();

        public ILogger Logger { get; private set; }

        public PhraseDictionaryManager(ILogger logger) : base()
        {
            Logger = logger;
        }

        /// <summary>
        /// Parse phrase data from a string and add it to the dictionary.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public bool AddPhrase(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }

            string[] parts = str.Split(' ');
            if (parts?.Length >= 3)
            {
                var key = parts[0]; // 片語字串
                var freq = Convert.ToInt32(parts[1]);

                // 如果片語已經存在表中，則比較頻率。若出現頻率大於或等於既有的片語，則覆蓋之。
                PhraseData phrase = null;
                if (PhraseDictionary.ContainsKey(key)) // 片語已經存在表中
                {
                    phrase = PhraseDictionary[key];
                    if (freq < phrase.Frequency)
                    {
                        return false;
                    }
                    phrase.BopomofoList.Clear();
                }
                else
                {
                    phrase = new PhraseData();
                    PhraseDictionary.Add(key, phrase);
                }

                phrase.Frequency = freq;
                for (int i = 2; i < parts.Length; i++)
                {
                    phrase.BopomofoList.Add(parts[i]);
                }
                Logger.Verbose("加入片語: {Key} {@Phrase}", key, phrase);
                return true;
            }
            Logger.Warning($"無效的片語: {str}");
            return false;
        }

        public void LoadFromEnumerable(IEnumerable<string> phraseLines)
        {
            foreach (var line in phraseLines)
            {
                AddPhrase(line);
            }
        }

        /// <summary>
        /// 將 libchewing 的 tsi.src 原始文字檔載入至字典結構。載入過程會丟棄重複的片語。
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public async Task LoadFromTextFileAsync(string filename)
        {
            if (!File.Exists(filename))
                throw new FileNotFoundException("指定的檔案不存在", filename);

            var startTime = DateTime.Now;
            Logger.Information($"Begin loading phrases from the file: {filename}");

            using (var reader = new StreamReader(filename, Encoding.UTF8))
            {
                string line = await reader.ReadLineAsync();
                while (line != null)
                {
                    AddPhrase(line);
                    line = await reader.ReadLineAsync();
                }
            }
            Logger.Information($"End loading phrases from the file: {filename}. Time spent: {DateTime.Now - startTime}");
            Logger.Information($"Totally loaded phrases: {PhraseDictionary.Count}");
        }

    }
}
