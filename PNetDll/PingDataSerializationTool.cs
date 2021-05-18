using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
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
        /// Serialize ping data to stream
        /// </summary>
        /// <param name="stream">Stream to which ping data should be serialized</param>
        /// <param name="pingData">Ping data </param>
        /// <returns></returns>
        public static Task SerializeAsync(Stream stream, PingData pingData)
        {
            return Task.Run(() =>
            {
                StreamWriter sw = new StreamWriter(stream);
                sw.WriteLine(JsonSerializer.Serialize(pingData));
            });            
        }

        /// <summary>
        /// Deserialize ping data from stream
        /// </summary>
        /// <param name="stream">Stream from which to read data</param>
        /// <returns>Ping data read from stream</returns>
        public static Task<PingData> DeserializeAsync(Stream stream, CancellationTokenSource cancellationToken = null)
        {
            CancellationToken token = new CancellationToken();
            if (cancellationToken != null)
                token = cancellationToken.Token;
            return Task.Run(() => {
                StreamReader sr = new StreamReader(stream);
                string line;
                while ((line = sr.ReadLine()) == null){
                    Task.Delay(1000);
                    token.ThrowIfCancellationRequested();
                }
                return JsonSerializer.Deserialize<PingData>(line);
            }, token);
        }
    }
}
