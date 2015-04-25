using System;

namespace Interfaces.Shared.Capabilities
{
	/// <summary>
	/// Represents a capability for a client
	/// This will be things like
	/// Screen avaliable, can play sounds
	/// Has admin rights
	/// What OS it can run etc.
	/// </summary>
	public interface ICapability
	{
		string Name { get; }

		bool HasCapability { get; }

		/// <summary>
		/// Some things are not yes/no
		/// Such as what OS it is
		/// </summary>
		/// <value>The capability value.</value>
		string CapabilityValue { get; }
	}
}

