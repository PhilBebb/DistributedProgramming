using System;
using System.Net.Sockets;
using System.IO;
using System.Threading.Tasks;
using Interfaces.Shared;
using System.Text;

namespace RemoteImplementations {
    internal static class InternalHelper {
        
        internal static void SendJson(string json, Stream stream) {
            if(string.IsNullOrWhiteSpace(json))
                return;

            Console.WriteLine("starting SendJson");

            byte[] buffer = GetBytes(json);
            stream.Write(buffer, 0, buffer.Length);
            stream.Flush();

            Console.WriteLine("wrote from SendJson");
        }

        internal static async Task SendJsonAsync(string json, Stream stream) {
            if(string.IsNullOrWhiteSpace(json))
                return;

            Console.WriteLine("starting SendJsonAsync");

            byte[] buffer = GetBytes(json);
            stream.Write(buffer, 0, buffer.Length);
            stream.Flush();

            Console.WriteLine("wrote from SendJsonAsync");
        }

        internal static async Task<IResult> ReadResultAsJsonAsync(Stream steam) {
            return await ReadJsonAsAsync<IResult>(steam, Helpers.Helper.JsonToResult);
        }

        internal static IResult ReadResultAsJson(Stream stream) {
            return ReadJsonAsAsync<IResult>(stream, Helpers.Helper.JsonToResult).Result;
        }

        internal static async Task<IJob> ReadJobAsJsonAsync(Stream stream) {
            return await ReadJsonAsAsync<IJob>(stream, Helpers.Helper.JsonToJob);
        }

        internal static async Task<T> ReadJsonAsAsync<T>(Stream stream, Func<string, T> conversion) {
            Console.WriteLine("starting Task<T> ReadJsonAsAsync<T>(...");

            int bufferReadSize = 10240;
            byte[] buffer = new byte[bufferReadSize];

            StringBuilder sb = new StringBuilder();
            Console.WriteLine("about to read");

            int readCount = 0;
            do {
                readCount = await stream.ReadAsync(buffer, 0, bufferReadSize);
                Console.WriteLine("readCount {0}", readCount);
                sb.Append(GetString(buffer));
            } while (readCount == bufferReadSize);

            Console.WriteLine("exit reading");
            return conversion.Invoke(sb.ToString());
        }

        internal static byte[] GetBytes(string str) {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        internal static string GetString(byte[] bytes) {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }
    }
}

