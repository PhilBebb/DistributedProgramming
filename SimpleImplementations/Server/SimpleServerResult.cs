using System;
using System.Collections.Generic;
using System.Linq;
using Interfaces.Server;
using Interfaces.Shared;

namespace SimpleImplementations {
	public class SimpleServerResult  :IServerResult {
		public SimpleServerResult (
			TimeSpan timeTaken, 
			IDictionary<IClient, IResult> clientResults,
			IJob request) {
			TimeTaken = timeTaken;
			ClientResults = clientResults;
			Request = request;
		}

		public TimeSpan TimeTaken {
			get;
			private set;
		}

		public IDictionary<IClient, IResult> ClientResults {
			get;
			private set;
		}

		public bool Success {
			get { return ClientResults.Any (r => r.Value.Success); }
		}

		public IDictionary<string, string> Result {
			get {
				var firstSuccess = ClientResults.Values.FirstOrDefault (r => r.Success);
				if (null == firstSuccess) {
					throw new InvalidOperationException ("No results");
				}

				return firstSuccess.Result;
			}
		}

		public IJob Request {
			get;
			private set;
		}

	}
}

