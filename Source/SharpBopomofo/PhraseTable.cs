using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace SharpBopomofo
{

    public class PhraseData
    {
        public int Frequency { get; set; } = 0;
        public List<string> BopomofoList = new List<string>();

        public override bool Equals(object obj)
        {
            var target = obj as PhraseData;
            if (target == null)
                return false;

            if (Frequency != target.Frequency || BopomofoList.Count != target.BopomofoList.Count)
                return false;
            
            for (int i = 0; i < BopomofoList.Count; i++)
            {
                if (BopomofoList[i] != target.BopomofoList[i])
                    return false;
            }
            return true;
        }
    }

    /// <summary>
    /// 此類別可將 libchewing 的 tsc.src 檔案載入至字典結構。
    /// 每個片語可能有多種注音字根，例如：
    /// 
    /// 一動不如一靜 2 ㄧ ㄉㄨㄥˋ ㄅㄨˋ ㄖㄨˊ ㄧ ㄐㄧㄥˋ
    /// 一動不如一靜 2 ㄧ ㄉㄨㄥˋ ㄅㄨˋ ㄖㄨˊ ㄧˊ ㄐㄧㄥˋ
    /// 一動不如一靜 2 ㄧˊ ㄉㄨㄥˋ ㄅㄨˋ ㄖㄨˊ ㄧ ㄐㄧㄥˋ
    /// 一動不如一靜 2 ㄧˊ ㄉㄨㄥˋ ㄅㄨˋ ㄖㄨˊ ㄧˊ ㄐㄧㄥˋ
    /// 
    /// 其中的 2 是出現頻率，數字越大代表越常出現，優先權越高。
    /// </summary>
    public class PhraseTable
    {
        private Dictionary<string, PhraseData> _table = new Dictionary<string, PhraseData>();

        public ILogger Logger { get; private set; }

        public PhraseTable(ILogger logger)
        {
            Logger = logger;
        }

        public bool AddPhrase(string line)
        {
            if (string.IsNullOrEmpty(line))
            {
                return false;
            }

            string[] parts = line.Split(' ');
            if (parts?.Length >= 3)
            {
                var key = parts[0]; // 片語字串
                var freq = Convert.ToInt32(parts[1]);

                // 如果片語已經存在表中，則比較頻率。若出現頻率大於或等於既有的片語，則覆蓋之。
                PhraseData phrase = null;
                if (_table.ContainsKey(key)) // 片語已經存在表中
                {
                    phrase = _table[key];
                    if (freq < phrase.Frequency)
                    {
                        return false;
                    }
                    phrase.BopomofoList.Clear();
                }
                else 
                {
                    phrase = new PhraseData();
                    _table.Add(key, phrase);
                }

                phrase.Frequency = freq;
                for (int i = 2; i < parts.Length; i++)
                {
                    phrase.BopomofoList.Add(parts[i]);
                }
                Logger.Verbose("加入片語: {Key} {@Phrase}", key, phrase);
                return true;
            }
            Logger.Warning($"無效的片語: {line}");
            return false;
        }

        public async Task LoadFromFileAsync(string filename)
        {
            using (var reader = new StreamReader(filename, Encoding.UTF8))
            {
                string line = await reader.ReadLineAsync();
                while (line != null)
                {
                    AddPhrase(line);
                    line = await reader.ReadLineAsync();
                }
            }
        }

        public bool ContainsPhrase(string phrase)
        {
            return _table.ContainsKey(phrase);
        }

        public PhraseData this[string key]
        {
            get
            {
                return _table[key];
            }
            private set
            {
                _table[key] = value;
            }
        }
    }
}
