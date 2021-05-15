using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PNetDll
{
    public class PingDataSerializationTool
    {
        public static void Serialize(Stream stream, PingData pingData)
        {
            StreamWriter sw = new StreamWriter(stream);
            sw.WriteLine(JsonSerializer.Serialize(pingData));
        }

        public static PingData Deserialize(Stream stream)
        {
            StreamReader sr = new StreamReader(stream);
            return JsonSerializer.Deserialize<PingData>(sr.ReadLine());
        }
    }
}
