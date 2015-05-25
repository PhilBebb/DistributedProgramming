using System;

namespace Interfaces.Shared.Capabilities {
	/// <summary>
	/// Represents a capability for a client
	/// This will be things like
	/// Screen avaliable, can play sounds
	/// Has admin rights
	/// What OS it can run etc.
	/// </summary>
	public abstract class ICapability : IEquatable<ICapability> , IComparable<ICapability> {

		#region comparable implementations

		public override int GetHashCode () {
			return Name.GetHashCode ();
		}

		public bool Equals (ICapability other) {
			return string.Equals (Name, other.Name)
			&& HasCapability == other.HasCapability
			&& string.Equals (CapabilityValue, other.CapabilityValue);
		}

		public int CompareTo (ICapability other) {
			return GetHashCode () - other.GetHashCode ();
		}

		#endregion

		public abstract string Name { get; set; }

		public virtual bool HasCapability { get; set; }

		/// <summary>
		/// Some things are not yes/no
		/// Such as what OS it is
		/// </summary>
		/// <value>The capability value.</value>
		public virtual string CapabilityValue { get; set; }
	}
}