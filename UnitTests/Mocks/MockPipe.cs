// PhilBebb
using System;
using RemoteImplementations.Pipelining;
using System.Threading.Tasks;

namespace UnitTests {
    public class MockPipe : Pipe {

        public Action _handleMethod;

        public MockPipe(Action handleMethod) {
            _handleMethod = handleMethod;
        }

        public override void HandleRequest(System.Net.Sockets.TcpClient tcpClient) {
            if(_handleMethod != null) {
                _handleMethod.Invoke();
            }
            base.HandleRequest(tcpClient);
        }

    }
}

