using System;
using System.Collections.Generic;
using Interfaces.Shared;

namespace TestImplimentation
{
	public class TestResult : IResult
	{
		public TestResult (bool success, IDictionary<string, string> result, IJob request)
		{
			Success = success;
			Result = result;
			Request = request;
		}

		public bool Success {
			get;
			private set;
		}

		public IDictionary<string, string> Result {
			get;
			private set;
		}

		public IJob Request {
			get;
			private set;
		}

	}
}

