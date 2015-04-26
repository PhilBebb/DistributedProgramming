using System;
using System.Net.Sockets;
using System.IO;
using System.Threading.Tasks;
using Interfaces.Shared;

namespace RemoteImplementations
{
	internal static class InternalHelper
	{
		internal static void SendJson (string json, Socket socket)
		{
			if (string.IsNullOrWhiteSpace (json))
				return;
			using (NetworkStream netSream = new NetworkStream (socket)) {
				using (StreamWriter streamWriter = new StreamWriter (netSream)) {
					streamWriter.WriteAsync (json);
					streamWriter.Flush ();
				}
			}
		}

		internal static async Task SendJsonAsync (string json, Socket socket)
		{
			if (string.IsNullOrWhiteSpace (json))
				return;
			using (NetworkStream netSream = new NetworkStream (socket)) {
				using (StreamWriter streamWriter = new StreamWriter (netSream)) {
					await streamWriter.WriteAsync (json);
					streamWriter.Flush ();
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
			using (NetworkStream netSream = new NetworkStream (socket)) {
				using (var streamReader = new StreamReader (netSream)) {
					var jsonResult = await streamReader.ReadToEndAsync ();
					return Helpers.Helper.JsonToJob (jsonResult);
				}
			}
		}
	}
}

