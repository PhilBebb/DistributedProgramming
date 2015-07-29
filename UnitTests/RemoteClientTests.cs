using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Interfaces.Shared;
using NUnit.Framework;

namespace UnitTests {

    [TestFixture()]
    public class RemoteClientTests {

        private int receivedCount = 0;
        private int proccessedCount = 0;

        private RemoteImplementations.Server server;
        private RemoteImplementations.RemoteClient client;

        private void client_RequestProcessed(object sender, RemoteImplementations.RequestProcessedEventArgs e) {
            proccessedCount++;
        }

        private void client_RequestReceived(object sender, RemoteImplementations.RequestReceivedEventArgs e) {
            receivedCount++;
        }

        [SetUp]
        public void Setup() {
            receivedCount = 0;
            proccessedCount = 0;
            server = RemoteServerTests.CreateServer();
            server.Start();

            client = new RemoteImplementations.RemoteClient();
            client.RequestReceived += client_RequestReceived;
            client.RequestProcessed += client_RequestProcessed;
        }

        [TearDown]
        public void TearDown() {
            //remember to kill the server
            RemoteServerTests.KillServer(server);
        }

        [Test]
        public void ClientCanReceiveMultipleCommands() {

            Assert.AreEqual(0, server.GetConnectedClients().Count());

            //wait for a request
            client.StartThreaded(IPAddress.Loopback, RemoteServerTests.ServerPort);

            //wait for connect, using Thread.Sleep was inconsitant
            int connectedCount = 0;
            for(int i = 0; i < 100; i++) {
                connectedCount = server.GetConnectedClients().Count();
                if(connectedCount > 0) {
                    break;
                }
            }

            Assert.AreEqual(1, connectedCount);

            IJob job = new MockJob();
            server.RubJob(job);
            server.RubJob(job);
            System.Threading.Thread.Sleep(250); //wait for stuff to happen

            Assert.AreEqual(2, receivedCount); //should be 2
            Assert.AreEqual(2, proccessedCount); //should be 2

        }

        [Test]
        public void StartThreadedCreatsAThreadAndDoesNotBlock() {
            Assert.IsFalse(client.IsRunningThreadded(), "Client is running in threading mode before it's been started");
            client.StartThreaded(IPAddress.Loopback, RemoteServerTests.ServerPort);
            bool runningThreaded = client.IsRunningThreadded();
            Assert.IsTrue(runningThreaded, "Client is not running in threading mode after been told to start");
            client.Stop();
            Assert.IsFalse(client.IsRunningThreadded(), "Client is running in threading mode after being told to stop");
        }
    }
}
