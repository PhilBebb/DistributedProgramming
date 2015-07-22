using System;
using System.Collections.Generic;
using System.Linq;
using Interfaces.Shared;
using Interfaces.Shared.Capabilities;
using System.Net.Sockets;
using System.IO;
using System.Threading.Tasks;
using RemoteImplementations.Networking;

namespace RemoteImplementations {

    /// <summary>
    /// An implimentation of IClient over a network
    /// Will require a RemoteClient on the other side to actually talk to
    /// </summary>
    public class RemoteNetworkedClient : IClient {
        private NetworkStateData _client = null;

        public RemoteNetworkedClient(NetworkStateData client, string name) {
            Name = name;
            _client = client;

            if(!_client.Client.Connected) {
                throw new InvalidOperationException("The client is not connected");
            }
        }

        public IResult RunJob(IJob job) {
            string jsonJob = Helpers.Helper.JobToJson(job);

            InternalHelper.SendJson(jsonJob, _client);

            var result = InternalHelper.ReadResultAsJson(_client);
            return result;
        }

        //		public async Task<IResult> RunJobAsync (IJob job)
        //		{
        //			string jsonJob = Helpers.Helper.JobToJson (job);
        //
        //			await InternalHelper.SendJsonAsync (jsonJob, _client.Client);
        //
        //			IResult result = await InternalHelper.ReadResultAsJsonAsync (_client.Client);
        //			return result;
        //		}

        public bool Ping() {
            return _client.Client.Connected;
        }

        public string Name {
            get;
            set;
        }

        public int Id {
            get;
            set;
        }

        public IEnumerable<ICapability> Capabilities {
            get;
            set;
        }

    }
}

