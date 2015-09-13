// PhilBebb
using System;
using RemoteImplementations;
using System.Threading.Tasks;

namespace UnitTests {
    public class MockRemoteClientBadAuth : RemoteClient {
        public MockRemoteClientBadAuth() {
        }

        override protected async Task<bool> SendAuthsync() {
            SendMessage("fail");
            return true;
        }
    }
}

