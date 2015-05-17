using System;
using System.Linq;
using Interfaces.Shared;

namespace UnitTests {
	public class MockClient : IClient , IEquatable<IClient> , IComparable<IClient> {
		#region comparable implementations

		public override int GetHashCode () {
			return Name.GetHashCode () + Id;
		}

		public bool Equals (IClient other) {
			return Id == other.Id && string.Equals (Name, other.Name);
		}

		public int CompareTo (IClient other) {
			return GetHashCode () - other.GetHashCode ();
		}

		#endregion

		public MockClient () : this (DateTime.UtcNow.ToString ()) {
		}

		public MockClient (string name) : this (name, DateTime.UtcNow.Millisecond) {
		}

		public MockClient (string name, int id) {
			Name = name;
			Id = id;
			PingOverride = true;
		}

		#region IClient implementation

		public IResult RunJob (IJob job) {
			if (null == RunJobOverride) {
				throw new InvalidOperationException ("No override for RunJob()!");
			}

			return RunJobOverride.Invoke (job);
		}

		public Func<IJob, IResult> RunJobOverride {
			get;
			set;
		}

		public bool Ping () {
			return PingOverride;
		}


		public bool PingOverride {
			get;
			set;
		}

		public string Name {
			get;
			set;
		}

		public int Id {
			get;
			set;
		}

		public System.Collections.Generic.IEnumerable<Interfaces.Shared.Capabilities.ICapability> Capabilities {
			get {
				throw new NotImplementedException ();
			}
		}

		#endregion
	}
}

