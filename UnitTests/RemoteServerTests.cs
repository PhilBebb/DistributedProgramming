using System;
using System.Linq;
using NUnit.Framework;
using Interfaces.Server;
using RemoteImplementations;
using Interfaces.Shared;
using System.Collections.Generic;
using System.Net;

namespace UnitTests {
    [TestFixture()]
    public class RemoteServerTests {
        internal const int ServerPort = 34554;

        private RemoteImplementations.Server server = null;

        internal static RemoteImplementations.Server CreateServer() {
            return new Server(ServerPort, new SimpleImplementations.SimpleClientSelection());
        }

        internal static void KillServer(RemoteImplementations.Server server) {
            server.Stop();
        }

        [SetUp]
        public void Setup() {
            server = CreateServer();
        }

        [TearDown]
        public void TearDown() {
            KillServer(server);
        }

        [Test]
        public void ServerCreatesTest() {
            //test logic goes here!
            KillServer(server);
        }

        [Test]
        public void AddClientTest() {

            //starts with no clients

            int expectedClientCount = 0;
            int clientCount = server.GetConnectedClients().Count();
            Assert.AreEqual(expectedClientCount, clientCount, "Expected {0} clinets after creating server, got {1}", expectedClientCount, clientCount);

            //adding a client actually adds the client

            IClient client1 = new MockClient("client1", expectedClientCount);
            Assert.True(server.AddClient(client1), "Failed to add the first client");
            //should now have 1 client
            expectedClientCount = 1;

            //check again
            clientCount = server.GetConnectedClients().Count();
            Assert.AreEqual(expectedClientCount, clientCount, "Expected {0} clinets after adding clients, got {1}", expectedClientCount, clientCount);

            //add again, expect a fail
            Assert.False(server.AddClient(client1), "Add the first client again");

            //add one more

            IClient client2 = new MockClient("client2", expectedClientCount);
            Assert.True(server.AddClient(client2), "Failed to add the second client");
            //should now have 1 client
            expectedClientCount = 2;

            //check again
            clientCount = server.GetConnectedClients().Count();
            Assert.AreEqual(expectedClientCount, clientCount, "Expected {0} clinets after adding clients, got {1}", expectedClientCount, clientCount);

            //and we are done!
            KillServer(server);
        }

        [Test]
        public void RemoveClientTest() {

            //add clients
            int expectedClientCount = 0;
            IClient client1 = new MockClient("client1", expectedClientCount);
            Assert.True(server.AddClient(client1), "Failed to add the first client");
            expectedClientCount++;

            IClient client2 = new MockClient("client2", expectedClientCount);
            Assert.True(server.AddClient(client2), "Failed to add the second client");
            expectedClientCount++;

            //check they are there
            int clientCount = server.GetConnectedClients().Count();
            Assert.AreEqual(expectedClientCount, clientCount, "Expected {0} clinets after adding clients, got {1}", expectedClientCount, clientCount);

            Assert.IsTrue(server.RemoveClient(client1), "Got false for removing existing client");
            Assert.IsFalse(server.RemoveClient(client1), "Got true for removing non existing client");
            Assert.IsTrue(server.RemoveClient(client2), "Got false for removing existing client");

            KillServer(server);
        }

        [Test]
        public void ConnectionAndPingTests() {

            MockClient pingTrue = new MockClient("pingTrue");

            //create a client that will ping false
            MockClient pingFalse = new MockClient("pingFalse") { PingOverride = false };

            //add clients
            server.AddClient(pingTrue);
            server.AddClient(pingFalse);

            int expectedCount = 1;
            var clients = server.GetConnectedClients().ToList();
            int actualCount = clients.Count();

            //check the GetConnectedClients method
            Assert.AreEqual(expectedCount, actualCount, "Expected {0} clinets after adding clients, got {1} not checking ping on clients", expectedCount, actualCount);
            Assert.IsTrue(clients.Contains(pingTrue), "pingTrue client not found in connected client list");
            Assert.IsFalse(clients.Contains(pingFalse), "pingFalse client found in connected client list");

            //check IsClientConnected method
            Assert.IsTrue(server.IsClientConnected(pingTrue), "pingTrue client not showing as connected");
            Assert.IsTrue(server.PingClient(pingTrue), "pingTrue client not pinging as expected");
            Assert.IsFalse(server.IsClientConnected(pingFalse), "pingTrue client not showing as connected");
            Assert.IsFalse(server.PingClient(pingFalse), "pingFalse client not pinging as expected");

            //set pingFalseToNowConnect
            pingFalse.PingOverride = true;
            expectedCount = 2;
            clients = server.GetConnectedClients().ToList();
            actualCount = clients.Count();
            Assert.AreEqual(expectedCount, actualCount, "Expected {0} clinets after adding clients, got {1} not checking ping on clients", expectedCount, actualCount);
            Assert.IsTrue(clients.Contains(pingTrue), "pingTrue client not found in connected client list");
            Assert.IsTrue(clients.Contains(pingFalse), "pingTrue client not found in connected client list");
            KillServer(server);
        }

        [Test]
        public void RunJobCallsClientsMethod() {

            bool simpleJobCalled = false;
            var simpleJobParams = new Dictionary<string, string> {
                { "param1", "val1" }, { "param2", "val2" }, { "param3", "val3" }, { "param4", "val4" }
            };

            MockClient simpleJobClient = new MockClient {
                RunJobOverride = (IJob job) => {
                    simpleJobCalled = true;
                    return new SimpleImplementations.SimpleResult(true, simpleJobParams, job);
                }
            };

            server.AddClient(simpleJobClient);
            IServerResult result = server.RubJob(new MockJob());
            Assert.IsNotNull(result, "Server result was null");

            //check the method actually ran
            Assert.IsTrue(simpleJobCalled, "The \"called\" variable was not set, so the client run job method was not called");

            //check results back
            Assert.IsTrue(result.ClientResults.ContainsKey(simpleJobClient), "ClientResults does not contain simpleJobClient");
            Assert.IsTrue(result.ClientResults[simpleJobClient].Success, "simpleJobClient result was not true");

            //check all the params are there
            foreach(string key in simpleJobParams.Keys) {
                Assert.AreEqual(simpleJobParams[key], result.ClientResults[simpleJobClient].Result[key], "The key {0} was not found in the dictionary", key);
            }

            KillServer(server);
        }

        [Test]
        public void AllClientsRunJob() {

            var clientsToRunResults = new Dictionary<int, bool>();

            var simpleJobParams = new Dictionary<string, string> {
                { "param1", "val1" }, { "param2", "val2" }, { "param3", "val3" }, { "param4", "val4" }
            };

            for(int i = 0; i < 10; i++) {
                var client = new MockClient(i.ToString(), i);
                //add to collection, set has rtun to false
                clientsToRunResults.Add(client.Id, true);

                client.RunJobOverride = (IJob job) => {
                    //now it's run, set to true
                    clientsToRunResults[client.Id] = true;
                    return new SimpleImplementations.SimpleResult(true, simpleJobParams, job);
                };
                server.AddClient(client);
            }

            IServerResult result = server.RubJob(new MockJob());

            foreach(var kvp in clientsToRunResults) {
                Assert.IsTrue(kvp.Value, "The client {0} did not run as expected");
            }
            Assert.IsTrue(result.Success, "A client did not report a success");

            KillServer(server);
        }

        [Test] //clients throwing an error does not bork the method
		public void ErorringClientdoesNotKillServerTest() {
            var server = CreateServer();
            var simpleJobParams = new Dictionary<string, string> {
                { "param1", "val1" }, { "param2", "val2" }, { "param3", "val3" }, { "param4", "val4" }
            };

            MockClient successfullClient = new MockClient {
                RunJobOverride = (IJob job) => {
                    return new SimpleImplementations.SimpleResult(true, simpleJobParams, job);
                }
            };

            MockClient failingClient = new MockClient {
                RunJobOverride = (IJob job) => {
                    throw new InvalidOperationException("TestError");
                }
            };

            server.AddClient(successfullClient);
            server.AddClient(failingClient);

            IServerResult result = server.RubJob(new MockJob());
            Assert.IsNotNull(result, "Server result was null");

            //check the method actually ran

            //check results back
            Assert.IsTrue(result.ClientResults.ContainsKey(successfullClient), "ClientResults does not contain successfullClient");
            Assert.IsTrue(result.ClientResults[successfullClient].Success, "successfullClient result was not true");

            Assert.IsTrue(result.ClientResults.ContainsKey(failingClient), "ClientResults does not contain failingClient");
            Assert.IsFalse(result.ClientResults[failingClient].Success, "failingClient result was true");

            KillServer(server);
        }

        //clients are run on diffirent threads
        [Test] //clients throwing an error does not bork the method
		public void JobsAreRunInParalellTest() {
            var simpleJobParams = new Dictionary<string, string> {
                { "param1", "val1" }, { "param2", "val2" }, { "param3", "val3" }, { "param4", "val4" }
            };

            TimeSpan shortTimespan = TimeSpan.FromMilliseconds(300);//needs to be long enough for tests to actuall run
            TimeSpan longTimespan = TimeSpan.FromMilliseconds(shortTimespan.TotalMilliseconds * 3);

            MockClient shortWaitClient1 = new MockClient {
                RunJobOverride = (IJob job) => {
                    System.Threading.Thread.Sleep(shortTimespan);
                    return new SimpleImplementations.SimpleResult(true, simpleJobParams, job);
                }
            };

            MockClient shortWaitClient2 = new MockClient {
                RunJobOverride = (IJob job) => {
                    System.Threading.Thread.Sleep(shortTimespan);
                    return new SimpleImplementations.SimpleResult(true, simpleJobParams, job);
                }
            };

            MockClient longWaitClient1 = new MockClient {
                RunJobOverride = (IJob job) => {
                    System.Threading.Thread.Sleep(longTimespan);
                    return new SimpleImplementations.SimpleResult(true, simpleJobParams, job);
                }
            };

            MockClient longWaitClient2 = new MockClient {
                RunJobOverride = (IJob job) => {
                    System.Threading.Thread.Sleep(longTimespan);
                    return new SimpleImplementations.SimpleResult(true, simpleJobParams, job);
                }
            };

            server.AddClient(shortWaitClient1);
            server.AddClient(shortWaitClient2);
            server.AddClient(longWaitClient1);
            server.AddClient(longWaitClient2);

            long resultTimeTaken = server.RubJob(new MockJob()).TimeTaken.Ticks;
            long threasholdTicks = (long)longTimespan.Ticks * 2; //should be less than the two long ones combined
            Assert.IsTrue(resultTimeTaken < threasholdTicks, "Server did not run tests in parallel");
            Assert.IsTrue(threasholdTicks > shortTimespan.Ticks, "Server time to run tests was shorter than expected");

            KillServer(server);
        }

        //        [Test] //test is no longer valid, statemachine is threaded

        //        public void StartThreadedCreatsAThreadAndDoesNotBlock() {
        //            Assert.IsFalse(server.IsRunningThreadded(), "Server is running in threading mode before it's been started");
        //            server.StartThreaded();
        //            Assert.IsTrue(server.IsRunningThreadded(), "Server is not running in threading mode after been told to start");
        //            server.Stop();
        //            Assert.IsFalse(server.IsRunningThreadded(), "Server is running in threading mode after being told to stop");
        //        }

        [Test]
        public void ServerCorrectlyHandlesAuthenticatingRemoteClient() {
            int expectedClientConnectedCount = server.GetConnectedClients().Count() + 1;
            server.Start();

            var client1 = new RemoteClient();
            client1.StartThreaded(IPAddress.Loopback, ServerPort);

            //wait for connection
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));

            int clientCountAfterConnectionAttempt = server.GetConnectedClients().Count();
            Assert.IsTrue(clientCountAfterConnectionAttempt == expectedClientConnectedCount);
        }

        [Test]
        public void ServerCanHandleMultipleClientsWhileThreading() {
            server.Start();

            var client1 = new RemoteClient();
            client1.StartThreaded(IPAddress.Loopback, ServerPort);

            var client2 = new RemoteClient();
            client2.StartThreaded(IPAddress.Loopback, ServerPort);

            bool client1Run = false;
            bool client2Run = false;

            client1.RequestProcessed += (sender, args) => {
                client1Run = true;
            };

            client2.RequestProcessed += (sender, args) => {
                client2Run = true;
            };
			
            //this apparently isn't enough time, set to 3 seconds and it passes
            //need to find out why
            System.Threading.Thread.Sleep(1000);

            Assert.AreEqual(2, server.GetConnectedClients().Count(), "Both servers have not connected");

            var result = server.RubJob(new MockJob());
            Assert.IsTrue(result.Success, "client did not mark job as success");
            Assert.IsTrue(client1Run, "client1 did not fire processed event");
            Assert.IsTrue(client2Run, "client2 did not fire processed event");

            KillServer(server);
        }
        //other tests to add
        //a lot more!
    }
}

