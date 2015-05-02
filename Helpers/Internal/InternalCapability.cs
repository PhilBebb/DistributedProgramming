﻿using System;
using Interfaces.Shared.Capabilities;
using Newtonsoft.Json.Converters;

namespace Helpers.Internal {
	internal class InternalCapability : ICapability {
		public InternalCapability () {
		}

		public InternalCapability (ICapability capability) {
			Name = capability.Name;
			HasCapability = capability.HasCapability;
			CapabilityValue = capability.CapabilityValue;
		}

		#region ICapability implementation

		public string Name {
			get;
			set;
		}

		public bool HasCapability {
			get;
			set;
		}

		public string CapabilityValue {
			get;
			set;
		}

		#endregion
	}

	internal class CapabilityConverter : CustomCreationConverter<ICapability> {
		public override ICapability Create (Type objectType) {
			return new InternalCapability ();
		}
	}

}

