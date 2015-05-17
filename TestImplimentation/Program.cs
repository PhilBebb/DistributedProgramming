using System;
using System.Collections.Generic;
using System.Linq;
using Interfaces.Shared;
using Interfaces.Shared.Capabilities;
using Interfaces.Server;
using RemoteImplementations;
using System.Threading.Tasks;

namespace TestImplimentation {
	class MainClass {
		public static void Main (string[] args) {
			//			RunTestServer ();
			RunTestNetworkServer ();
		}

		static void RunTestNetworkServer () {

			const int serverPort = 63564;

			Server remoteServer = new Server (serverPort, new SimpleImplementations.SimpleClientSelection ());
			//remoteServer.Start ();
			Task.Factory.StartNew (remoteServer.Start);

			while (!remoteServer.GetConnectedClients ().Any ()) {
				System.Threading.Thread.Sleep (100);
			}

			IJob job = new TestJob ("test job 1", 1, null, null);

			var result = remoteServer.RubJob (job);
			Console.WriteLine ("The result was {0}", result.Success);
			Console.WriteLine (string.Join (Environment.NewLine, result.Result.Select (r => string.Format ("{0} : {1}", r.Key, r.Value))));

			job = new TestJob ("test job 2", 2, null, null);
			result = remoteServer.RubJob (job);
			Console.WriteLine ("The result was {0}", result.Success);
			Console.WriteLine (string.Join (Environment.NewLine, result.Result.Select (r => string.Format ("{0} : {1}", r.Key, r.Value))));

			job = new TestJob ("test job 3", 3, null, null);
			result = remoteServer.RubJob (job);
			Console.WriteLine ("The result was {0}", result.Success);
			Console.WriteLine (string.Join (Environment.NewLine, result.Result.Select (r => string.Format ("{0} : {1}", r.Key, r.Value))));

			Console.WriteLine ("Done");
			Console.ReadLine ();
		}

		static void RunTestServer () {
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
