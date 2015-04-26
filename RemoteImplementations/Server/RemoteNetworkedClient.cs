using System;
using System.Collections.Generic;
using System.Linq;
using Interfaces.Shared;
using Interfaces.Shared.Capabilities;
using System.Net.Sockets;
using System.IO;
using System.Threading.Tasks;

namespace RemoteImplementations
{

	/// <summary>
	/// An implimentation of IClient over a network
	/// Will require a RemoteClient on the other side to actually talk to
	/// </summary>
	public class RemoteNetworkedClient : IClient
	{
		private TcpClient _client = null;

		public RemoteNetworkedClient (TcpClient client, string name)
		{
			Name = name;
			if (!_client.Connected) {
				throw new InvalidOperationException ("The client is not connected");
			}
			_client = client;
		}

		public IResult RunJob (IJob job)
		{
			string jsonJob = Helpers.Helper.JobToJson (job);

			SendJson (jsonJob);

			var result = ReadJson ();
			return result;
		}

		public async Task<IResult> RunJobAsync (IJob job)
		{
			string jsonJob = Helpers.Helper.JobToJson (job);

			await SendJsonAsync (jsonJob);

			IResult result = await ReadJsonAsync ();
			return result;
		}

		private void SendJson (string json)
		{
			if (string.IsNullOrWhiteSpace (json))
				return;
			using (NetworkStream netSream = _client.GetStream ()) {
				using (StreamWriter streamWriter = new StreamWriter (_client.GetStream ())) {
					streamWriter.WriteAsync (json);
					streamWriter.Flush ();
				}
			}
		}

		private async Task SendJsonAsync (string json)
		{
			if (string.IsNullOrWhiteSpace (json))
				return;
			using (NetworkStream netSream = _client.GetStream ()) {
				using (StreamWriter streamWriter = new StreamWriter (_client.GetStream ())) {
					await streamWriter.WriteAsync (json);
					streamWriter.Flush ();
				}
			}
		}

		private async Task<IResult> ReadJsonAsync ()
		{
			using (NetworkStream netSream = _client.GetStream ()) {
				using (var streamReader = new StreamReader (netSream)) {
					var jsonResult = await streamReader.ReadToEndAsync ();
					return Helpers.Helper.JsonToResult (jsonResult);
				}
			}
		}

		private IResult ReadJson ()
		{
			using (NetworkStream netSream = _client.GetStream ()) {
				using (var streamReader = new StreamReader (netSream)) {
					string jsonResult = streamReader.ReadToEnd ();
					return Helpers.Helper.JsonToResult (jsonResult);
				}
			}
		}

		public bool Ping ()
		{
			return _client.Connected;
		}

		public string Name {
			get;
			set;
		}

		public int Id {
			get;
			set;
		}

		public IEnumerable<ICapability> Capabilities {
			get;
			set;
		}

	}
}

