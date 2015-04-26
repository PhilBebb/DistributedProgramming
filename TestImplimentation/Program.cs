using System;
using System.Collections.Generic;
using System.Linq;
using Interfaces.Shared;
using Interfaces.Shared.Capabilities;
using Interfaces.Server;

namespace TestImplimentation
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Lets go!");

			IServer server = new TestServer ();

			IClient client = new TestClient ();

			server.AddClient (client);

			IJob job = new TestJob ("Test", 23, new List<ICapability> (), new Dictionary<string, string> ());

			IServerResult serverResult = server.RubJob (job);

			Console.WriteLine (serverResult.Success);
			foreach (string x in serverResult.Result.Keys) {
				Console.WriteLine ("{0} = {1}", x, serverResult.Result [x]);
			}
		}
	}
}
