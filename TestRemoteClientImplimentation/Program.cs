using System;
using System.Net;

namespace TestRemoteClientImplimentation
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			const int serverPort = 63564;

			Console.WriteLine ("Starting Client");
			RemoteImplementations.RemoteClient client = new RemoteImplementations.RemoteClient ();
			client.Start (IPAddress.Loopback, serverPort);
			Console.ReadLine ();
		}
	}
}
