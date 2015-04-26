using System;
using System.Collections.Generic;
using System.Linq;
using Interfaces;
using Interfaces.Shared;
using Interfaces.Shared.Capabilities;

namespace TestImplimentation
{
	public class TestClient : IClient
	{
		public TestClient () : this (new Random ().Next (), "TestClient")
		{
		}

		public TestClient (int id, string name)
		{
			Name = name;
			Id = id;
		}

		public IResult RunJob (IJob job)
		{
			return new TestResult (
				true,
				new Dictionary<string, string>{ { "param1", "p1Val" }, { "param2", "p2Val" } }
				, job);
		}

		public bool Ping ()
		{
			return true;
		}

		public string Name {
			get;
			set;
		}

		public int Id {
			get;
			private set;
		}

		public IEnumerable<ICapability> Capabilities {
			get {
				return new List<ICapability> ();
			}
		}
	}
}

