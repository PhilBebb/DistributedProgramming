using System;
using System.Linq;
using Interfaces.Shared;
using Helpers.Internal;
using System.Collections.Generic;
using Interfaces.Shared.Capabilities;
using System.Runtime.CompilerServices;

namespace Helpers {
    public static class Helper {
        public static bool IsWindows {
            get {
                //shamelessly taken from Mono
                PlatformID platform = Environment.OSVersion.Platform;
                return platform == PlatformID.Win32S || platform == PlatformID.Win32Windows || platform == PlatformID.Win32NT || platform == PlatformID.WinCE;
            }
        }

        public static string JobToJson(IJob job) {
            return Newtonsoft.Json.JsonConvert.SerializeObject(job);
        }

        public static IJob JsonToJob(string jobJson) {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<InternalJob>(jobJson, 
                new ResultConverter(),
                new JobConverter(),
                new CapabilityConverter()
            );
        }

        public static string ResultToJson(IResult result) {
            return Newtonsoft.Json.JsonConvert.SerializeObject(result);
        }

        public static IResult JsonToResult(string resultJson) {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<InternalResult>(resultJson, 
                new ResultConverter(),
                new JobConverter(),
                new CapabilityConverter()
            );
		
        }

        public static IList<IClient> GetPossibleClients(IEnumerable<IClient> clients, IEnumerable<ICapability> capabilities) {
            return clients.Where(c => {
                return capabilities.All(cap => c.Capabilities.Contains(cap));
            }).ToList();
        }

        public static void Log(string message, [CallerMemberName] string callername = "") {
            System.Console.WriteLine(
                "[{0}] - Thread-{1}- {2}",
                callername,
                System.Threading.Thread.CurrentThread.ManagedThreadId, message);
        }

        public static void Log(Exception ex, [CallerMemberName] string callername = "") {
            System.Console.WriteLine(
                "[{0}] - Thread-{1}- {2}",
                callername,
                System.Threading.Thread.CurrentThread.ManagedThreadId, ex.Message);
        }
    }
}