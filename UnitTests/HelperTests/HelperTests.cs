using System;
using NUnit.Framework;
using System.Collections.Generic;
using Interfaces.Shared.Capabilities;

namespace UnitTests {

	[TestFixture]
	public class HelperTests {

		[Test]
		public void GetPossibleClientsOnlyReturnsCorrectClients () {

			var cap1 = new MockCapability ("cap1", true);
			var cap2 = new MockCapability ("cap2", true);
			var cap3 = new MockCapability ("cap3", true);
			var cap4 = new MockCapability ("cap4", true);
			var cap5 = new MockCapability ("cap5", true);

			var mock1 = new MockClient {
				Capabilities = new List<ICapability> {
					cap1, cap2, cap3
				}
			};

			var mock2 = new MockClient {
				Capabilities = new List<ICapability> {
					cap2, cap3, cap4
				}
			};

			var mock3 = new MockClient {
				Capabilities = new List<ICapability> {
					cap3, cap4, cap5
				}			
			};

			var deps1 = new List<ICapability> {
				cap2, cap3 //mock1 and 2
			};

			var deps2 = new List<ICapability> {
				cap4, cap5 //mock 3
			};

			var deps3 = new List<ICapability> {
				cap1, cap2, cap3, cap4, cap5 //none
			};

			var clients = new List<MockClient> { mock1, mock2, mock3 };

			var clientResults1 = Helpers.Helper.GetPossibleClients (clients, deps1);
			Assert.IsTrue (clientResults1.Contains (mock1));
			Assert.IsTrue (clientResults1.Contains (mock2));
			Assert.IsFalse (clientResults1.Contains (mock3));

			var clientResults2 = Helpers.Helper.GetPossibleClients (clients, deps2);
			Assert.IsFalse (clientResults2.Contains (mock1));
			Assert.IsFalse (clientResults2.Contains (mock2));
			Assert.IsTrue (clientResults2.Contains (mock3));

			var clientResults3 = Helpers.Helper.GetPossibleClients (clients, deps3);
			Assert.IsFalse (clientResults3.Contains (mock1));
			Assert.IsFalse (clientResults3.Contains (mock2));
			Assert.IsFalse (clientResults3.Contains (mock3));
		}

	}
}

