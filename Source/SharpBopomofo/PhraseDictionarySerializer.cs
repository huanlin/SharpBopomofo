using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using ProtoBuf;
using Serilog;

namespace SharpBopomofo
{
    public class PhraseDictionarySerializer
    {
        public static void SerializeToBinaryFile(string filename, PhraseBopomofoDictionary dictionary)
        {
            var startTime = DateTime.Now;
            Log.Logger.Information($"開始將字典序列化至 bin 檔案：{filename}");

            using (var file = File.Create(filename))
            {
                Serializer.Serialize(file, dictionary);
                file.Flush();
            }

            Log.Logger.Information($"成功將字典序列化至 bin 檔案: {filename}。片語數量：{dictionary.Count}。費時：{DateTime.Now - startTime}");
        }

        public static PhraseBopomofoDictionary DeserializeFromBinaryFile(string filename)
        {
            var startTime = DateTime.Now;
            Log.Logger.Information($"開始將 bin 檔案反序列化至記憶體：{filename}");

            using (var file = File.OpenRead(filename))
            {
                var result = Serializer.Deserialize<PhraseBopomofoDictionary>(file);

                Log.Logger.Information($"成功將 bin 檔案反序列化至記憶體: {filename}。片語數量：{result.Count}。費時：{DateTime.Now - startTime}");
                return result;
            }
        }

        public static void SerializeToJsonFile(string filename, PhraseBopomofoDictionary dictionary)
        {
            var startTime = DateTime.Now;
            Log.Logger.Information($"開始將字典序列化至 json 檔案：{filename}");

            using (var writer = new StreamWriter(filename, false, Encoding.UTF8))
            {
                var serializer = JsonSerializer.Create();
                serializer.Serialize(writer, dictionary, typeof(PhraseBopomofoDictionary));
                writer.Flush();
            }

            Log.Logger.Information($"成功將字典序列化至 json 檔案: {filename}。片語數量：{dictionary.Count}。費時：{DateTime.Now - startTime}");
        }

        public static PhraseBopomofoDictionary DeserializeFromJsonFile(string filename)
        {
            var startTime = DateTime.Now;
            Log.Logger.Information($"開始將 json 檔案反序列化至記憶體：{filename}");

            using (var reader = new StreamReader(filename, Encoding.UTF8))
            {
                var serializer = new JsonSerializer();
                var result = (PhraseBopomofoDictionary) serializer.Deserialize(reader, typeof(PhraseBopomofoDictionary));

                Log.Logger.Information($"成功將 json 檔案反序列化至記憶體: {filename}。片語數量：{result.Count}。費時：{DateTime.Now - startTime}");
                return result;
            }
        }

    }
}
