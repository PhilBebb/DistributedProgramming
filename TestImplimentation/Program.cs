using System;
using System.Collections.Generic;
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

			Console.Write (serverResult.Success);
		}
	}
}
