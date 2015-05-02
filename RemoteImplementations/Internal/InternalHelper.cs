using System;
using System.Net.Sockets;
using System.IO;
using System.Threading.Tasks;
using Interfaces.Shared;
using System.Text;

namespace RemoteImplementations {
	internal static class InternalHelper {
		internal static void SendJson (string json, TcpClient socket) {
			if (string.IsNullOrWhiteSpace (json))
				return;

			Console.WriteLine ("starting SendJson");

			var netStream = socket.GetStream ();
			byte[] buffer = GetBytes (json);
			netStream.Write (buffer, 0, buffer.Length);
			netStream.Flush ();

			Console.WriteLine ("wrote from SendJson");
		}

		internal static async Task SendJsonAsync (string json, TcpClient socket) {
			if (string.IsNullOrWhiteSpace (json))
				return;

			Console.WriteLine ("starting SendJsonAsync");

			var netStream = socket.GetStream ();
			byte[] buffer = GetBytes (json);
			netStream.Write (buffer, 0, buffer.Length);
			netStream.Flush ();

			Console.WriteLine ("wrote from SendJsonAsync");
		}

		internal static async Task<IResult> ReadResultAsJsonAsync (TcpClient socket) {
			return await ReadJsonAsAsync<IResult> (socket, Helpers.Helper.JsonToResult);
		}

		internal static IResult ReadResultAsJson (TcpClient socket) {
			return ReadJsonAsAsync<IResult> (socket, Helpers.Helper.JsonToResult).Result;
		}

		internal static async Task<IJob> ReadJobAsJsonAsync (TcpClient socket) {
			return await ReadJsonAsAsync<IJob> (socket, Helpers.Helper.JsonToJob);
		}

		internal static async Task<T> ReadJsonAsAsync<T> (TcpClient socket, Func<string, T> conversion) {
			Console.WriteLine ("starting Task<T> ReadJsonAsAsync<T>(...");
			Console.WriteLine ("Got net stream");
			Console.WriteLine ("Got stream reader");

			int bufferReadSize = 10240;
			byte[] buffer = new byte[bufferReadSize];

			StringBuilder sb = new StringBuilder ();
			Console.WriteLine ("about to read");

			int readCount = 0;
			do {
				readCount = await socket.GetStream ().ReadAsync (buffer, 0, bufferReadSize);
				Console.WriteLine ("readCount {0}", readCount);
				sb.Append (GetString (buffer));
			} while (readCount == bufferReadSize);

			Console.WriteLine ("exit reading");
			return conversion.Invoke (sb.ToString ());
		}

		internal static byte[] GetBytes (string str) {
			byte[] bytes = new byte[str.Length * sizeof(char)];
			System.Buffer.BlockCopy (str.ToCharArray (), 0, bytes, 0, bytes.Length);
			return bytes;
		}

		internal static string GetString (byte[] bytes) {
			char[] chars = new char[bytes.Length / sizeof(char)];
			System.Buffer.BlockCopy (bytes, 0, chars, 0, bytes.Length);
			return new string (chars);
		}
	}
}

