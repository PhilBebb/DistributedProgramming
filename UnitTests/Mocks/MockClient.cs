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
			OveridePingResult = true;
		}

		#region IClient implementation

		public IResult RunJob (IJob job) {
			throw new NotImplementedException ();
		}

		public bool Ping () {
			return OveridePingResult;
		}


		public bool OveridePingResult {
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

