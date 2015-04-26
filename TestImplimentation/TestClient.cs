using System;
using System.Collections.Generic;
using System.Linq;
using Interfaces.Shared;
using Interfaces.Shared.Capabilities;
using System.Threading.Tasks;

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
			return new SimpleImplementations.SimpleResult (
				true,
				new Dictionary<string, string>{ { "param1", "p1Val" }, { "param2", "p2Val" } }
				, job);
		}


		public async Task<IResult> RunJobAsync (IJob job)
		{
			return new SimpleImplementations.SimpleResult (
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

