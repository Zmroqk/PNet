using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PNetDll
{
    /// <summary>
    /// Tools for serializing and deserializing PingData
    /// </summary>
    public class PingDataSerializationTool
    {
        /// <summary>
        /// Serialize ping data to stream
        /// </summary>
        /// <param name="stream">Stream to which ping data should be serialized</param>
        /// <param name="pingData">Ping data </param>
        public static void Serialize(Stream stream, PingData pingData)
        {
            StreamWriter sw = new StreamWriter(stream);
            sw.WriteLine(JsonSerializer.Serialize(pingData));
        }

        /// <summary>
        /// Deserialize ping data from stream
        /// </summary>
        /// <param name="stream">Stream from which to read data</param>
        /// <returns>Ping data read from stream</returns>
        public static PingData Deserialize(Stream stream)
        {
            StreamReader sr = new StreamReader(stream);
            return JsonSerializer.Deserialize<PingData>(sr.ReadLine());
        }
    }
}
