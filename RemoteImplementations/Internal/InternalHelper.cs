using System;
using System.Net.Sockets;
using System.IO;
using System.Threading.Tasks;
using Interfaces.Shared;
using System.Text;

namespace RemoteImplementations
{
	internal static class InternalHelper
	{
		internal static void SendJson (string json, Socket socket)
		{
			if (string.IsNullOrWhiteSpace (json))
				return;

			Console.WriteLine ("starting SendJson");

			using (NetworkStream netSream = new NetworkStream (socket)) {
				using (StreamWriter streamWriter = new StreamWriter (netSream)) {
					streamWriter.WriteAsync (json);
					streamWriter.Flush ();
//					streamWriter.WriteLine ();
//					streamWriter.WriteLine ();
//					streamWriter.Flush ();
					Console.WriteLine ("wrote from SendJson");
				}
			}
		}

		internal static async Task SendJsonAsync (string json, Socket socket)
		{
			if (string.IsNullOrWhiteSpace (json))
				return;
			Console.WriteLine ("starting SendJsonAsync");

			using (NetworkStream netSream = new NetworkStream (socket)) {
				using (StreamWriter streamWriter = new StreamWriter (netSream)) {
					await streamWriter.WriteAsync (json);
					streamWriter.Flush ();
//					streamWriter.WriteLine ();
//					streamWriter.WriteLine ();
//					streamWriter.Flush ();
					Console.WriteLine ("wrote from SendJsonAsync");
				}
			}
		}

		internal static async Task<IResult> ReadResultAsJsonAsync (Socket socket)
		{
			using (NetworkStream netSream = new NetworkStream (socket)) {
				using (var streamReader = new StreamReader (netSream)) {
					var jsonResult = await streamReader.ReadToEndAsync ();
					return Helpers.Helper.JsonToResult (jsonResult);
				}
			}
		}

		internal static IResult ReadResultAsJson (Socket socket)
		{
			using (NetworkStream netSream = new NetworkStream (socket)) {
				using (var streamReader = new StreamReader (netSream)) {
					string jsonResult = streamReader.ReadToEnd ();
					return Helpers.Helper.JsonToResult (jsonResult);
				}
			}
		}

		internal static async Task<IJob> ReadJobAsJsonAsync (Socket socket)
		{
			return await ReadJsonAsAsync<IJob> (socket, Helpers.Helper.JsonToJob);
		}

		internal static async Task<T> ReadJsonAsAsync<T> (Socket socket, Func<string, T> conversion)
		{
			Console.WriteLine ("starting Task<T> ReadJsonAsAsync<T>(...");
			using (NetworkStream netSream = new NetworkStream (socket)) {
				Console.WriteLine ("Got net stream");
				using (var streamReader = new StreamReader (netSream)) {
					Console.WriteLine ("Got stream reader");

					int bufferReadSize = 10240;
					char[] buffer = new char[bufferReadSize];
					StringBuilder sb = new StringBuilder ();
					Console.WriteLine ("about to read");
					int readCount = 0;
					do {
						readCount = await streamReader.ReadAsync (buffer, 0, bufferReadSize);
						Console.WriteLine ("readCount {0}", readCount);
						sb.Append (buffer);
					} while (readCount == bufferReadSize);
					Console.WriteLine ("exit reading");
					return conversion.Invoke (sb.ToString ());
				}
			}
		}
	}
}

