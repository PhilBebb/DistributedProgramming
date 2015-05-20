using System;
using System.Linq;
using NUnit.Framework;
using Interfaces.Server;
using RemoteImplementations;
using Interfaces.Shared;
using System.Collections.Generic;

namespace UnitTests {
	[TestFixture ()]
	public class RemoteServerTests {
		static RemoteImplementations.Server CreateServer () {
			return new Server (1000, new SimpleImplementations.SimpleClientSelection ());
		}

		static void KillServer (RemoteImplementations.Server server) {
			server.Stop ();
		}

		[Test]
		public void ServerCreatesTest () {
			var server = CreateServer ();
			//test logic goes here!
			KillServer (server);
		}

		[Test]
		public void AddClientTest () {
			var server = CreateServer ();

			//starts with no clients

			int expectedClientCount = 0;
			int clientCount = server.GetConnectedClients ().Count ();
			Assert.AreEqual (expectedClientCount, clientCount, "Expected {0} clinets after creating server, got {1}", expectedClientCount, clientCount);

			//adding a client actually adds the client

			IClient client1 = new MockClient ("client1", expectedClientCount);
			Assert.True (server.AddClient (client1), "Failed to add the first client");
			//should now have 1 client
			expectedClientCount = 1;

			//check again
			clientCount = server.GetConnectedClients ().Count ();
			Assert.AreEqual (expectedClientCount, clientCount, "Expected {0} clinets after adding clients, got {1}", expectedClientCount, clientCount);

			//add again, expect a fail
			Assert.False (server.AddClient (client1), "Add the first client again");

			//add one more

			IClient client2 = new MockClient ("client2", expectedClientCount);
			Assert.True (server.AddClient (client2), "Failed to add the second client");
			//should now have 1 client
			expectedClientCount = 2;

			//check again
			clientCount = server.GetConnectedClients ().Count ();
			Assert.AreEqual (expectedClientCount, clientCount, "Expected {0} clinets after adding clients, got {1}", expectedClientCount, clientCount);

			//and we are done!
			KillServer (server);
		}

		[Test]
		public void RemoveClientTest () {
			var server = CreateServer ();

			//add clients
			int expectedClientCount = 0;
			IClient client1 = new MockClient ("client1", expectedClientCount);
			Assert.True (server.AddClient (client1), "Failed to add the first client");
			expectedClientCount++;

			IClient client2 = new MockClient ("client2", expectedClientCount);
			Assert.True (server.AddClient (client2), "Failed to add the second client");
			expectedClientCount++;

			//check they are there
			int clientCount = server.GetConnectedClients ().Count ();
			Assert.AreEqual (expectedClientCount, clientCount, "Expected {0} clinets after adding clients, got {1}", expectedClientCount, clientCount);

			Assert.IsTrue (server.RemoveClient (client1), "Got false for removing existing client");
			Assert.IsFalse (server.RemoveClient (client1), "Got true for removing non existing client");
			Assert.IsTrue (server.RemoveClient (client2), "Got false for removing existing client");

			KillServer (server);
		}

		[Test]
		public void ConnectionAndPingTests () {
			var server = CreateServer ();

			MockClient pingTrue = new MockClient ("pingTrue");

			//create a client that will ping false
			MockClient pingFalse = new MockClient ("pingFalse"){ PingOverride = false };

			//add clients
			server.AddClient (pingTrue);
			server.AddClient (pingFalse);

			int expectedCount = 1;
			var clients = server.GetConnectedClients ().ToList ();
			int actualCount = clients.Count ();

			//check the GetConnectedClients method
			Assert.AreEqual (expectedCount, actualCount, "Expected {0} clinets after adding clients, got {1} not checking ping on clients", expectedCount, actualCount);
			Assert.IsTrue (clients.Contains (pingTrue), "pingTrue client not found in connected client list");
			Assert.IsFalse (clients.Contains (pingFalse), "pingFalse client found in connected client list");

			//check IsClientConnected method
			Assert.IsTrue (server.IsClientConnected (pingTrue), "pingTrue client not showing as connected");
			Assert.IsTrue (server.PingClient (pingTrue), "pingTrue client not pinging as expected");
			Assert.IsFalse (server.IsClientConnected (pingFalse), "pingTrue client not showing as connected");
			Assert.IsFalse (server.PingClient (pingFalse), "pingFalse client not pinging as expected");

			//set pingFalseToNowConnect
			pingFalse.PingOverride = true;
			expectedCount = 2;
			clients = server.GetConnectedClients ().ToList ();
			actualCount = clients.Count ();
			Assert.AreEqual (expectedCount, actualCount, "Expected {0} clinets after adding clients, got {1} not checking ping on clients", expectedCount, actualCount);
			Assert.IsTrue (clients.Contains (pingTrue), "pingTrue client not found in connected client list");
			Assert.IsTrue (clients.Contains (pingFalse), "pingTrue client not found in connected client list");
			KillServer (server);
		}

		[Test]
		public void RunJobCallsClientsMethod () {
			var server = CreateServer ();

			bool simpleJobCalled = false;
			var simpleJobParams = new Dictionary<string,string> {
				{ "param1", "val1" }, { "param2", "val2" }, { "param3", "val3" }, { "param4", "val4" }
			};

			MockClient simpleJobClient = new MockClient { 
				RunJobOverride = (IJob job) => {
					simpleJobCalled = true;
					return new SimpleImplementations.SimpleResult (true, simpleJobParams, job);
				}
			};

			server.AddClient (simpleJobClient);
			IServerResult result = server.RubJob (new MockJob ());
			Assert.IsNotNull (result, "Server result was null");

			//check the mothod actually ran
			Assert.IsTrue (simpleJobCalled, "The \"called\" variable was not set, so the client run job method was not called");

			//check results back
			Assert.IsTrue (result.ClientResults.ContainsKey (simpleJobClient), "ClientResults does not contain simpleJobClient");
			Assert.IsTrue (result.ClientResults [simpleJobClient].Success, "simpleJobClient result was not true");

			//check all the params are there
			foreach (string key in simpleJobParams.Keys) {
				Assert.AreEqual (simpleJobParams [key], result.ClientResults [simpleJobClient].Result [key], "The key {0} was not found in the dictionary", key);
			}

			KillServer (server);
		}

		[Test]
		public void AllClientsRunJob () {
			var server = CreateServer ();

			var clientsToRunResults = new Dictionary<int, bool> ();

			var simpleJobParams = new Dictionary<string,string> {
				{ "param1", "val1" }, { "param2", "val2" }, { "param3", "val3" }, { "param4", "val4" }
			};

			for (int i = 0; i < 10; i++) {
				var client = new MockClient (i.ToString (), i);
				//add to collection, set has rtun to false
				clientsToRunResults.Add (client.Id, true);

				client.RunJobOverride = (IJob job) => {
					//now it's run, set to true
					clientsToRunResults [client.Id] = true;
					return new SimpleImplementations.SimpleResult (true, simpleJobParams, job);
				};
				server.AddClient (client);
			}

			IServerResult result = server.RubJob (new MockJob ());

			foreach (var kvp in clientsToRunResults) {
				Assert.IsTrue (kvp.Value, "The client {0} did not run as expected");
			}
			Assert.IsTrue (result.Success, "A client did not report a success");

			KillServer (server);
		}

		//mix of success and fails
		[Test]
		public void ServerCreatesTest () {
			var server = CreateServer ();
			//test logic goes here!
			KillServer (server);
		}


		//other tests to add
		//clients throwing an error does not bork the method
		//a lot more!
	}
}

