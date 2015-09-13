// PhilBebb
using System;
using NUnit.Framework;
using RemoteImplementations.Pipelining;

namespace UnitTests {

    [TestFixture()]
    public class PipelineTests {
        public PipelineTests() {
        }

        [SetUp]
        public void Setup() {
        }

        [TearDown]
        public void TearDown() {
        }

        [Test]
        public void BasicPipelineTest() {

            bool pipe1Run = false;
            bool pipe2Run = false;
            bool pipe3Run = false;
            bool pipe4Run = false;

            Pipe pipe1 = new MockPipe(() => {
                pipe1Run = true;
            });
            Pipe pipe2 = new MockPipe(() => {
                pipe2Run = true;
            });
            Pipe pipe3 = new MockPipe(() => {
                pipe3Run = true;
            });
            Pipe pipe4 = new MockPipe(() => {
                pipe4Run = true;
            });

            pipe1.Next = pipe2;
            pipe2.Next = pipe3;
            pipe3.Next = pipe4;

            pipe1.HandleRequest(null);

            Assert.IsTrue(pipe1Run);
            Assert.IsTrue(pipe2Run);
            Assert.IsTrue(pipe3Run);
            Assert.IsTrue(pipe4Run);
        }
    }
}

