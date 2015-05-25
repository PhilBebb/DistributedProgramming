using System;
using NUnit.Framework;
using SimpleImplementations;
using System.Collections.Generic;
using Interfaces.Shared;

namespace UnitTests {
	[TestFixture]
	public class SimpleServerResultsTest {
		[Test]
		public void SingleSuccesfulResultIsOveralSuccess () {
			//job
			var request = new MockJob ();

			//params back are not important
			var clientResponseResults = new Dictionary<string, string> ();

			//clients and results
			var client1 = new MockClient ();
			var result1 = new SimpleResult (true, clientResponseResults, request);

			//set results
			var resultDictionary = new Dictionary<IClient, IResult> ();
			resultDictionary.Add (client1, result1);

			//overall result
			var simpleResult = new SimpleServerResult (TimeSpan.Zero, resultDictionary, request);

			Assert.IsTrue (simpleResult.Success, "The simple result did not report true when one of the client results where true");
		}

		[Test]
		public void AllSuccesfulResultIsOveralSuccess () {
			//job
			var request = new MockJob ();

			//params back are not important
			var clientResponseResults = new Dictionary<string, string> ();

			var resultDictionary = new Dictionary<IClient, IResult> ();

			//clients and results
			//add multiple clients, all with success
			for (int i = 0; i < 10; i++) {
				//clients
				var client = new MockClient ();
				var result = new SimpleResult (true, clientResponseResults, request);

				//set results
				resultDictionary.Add (client, result);
			}

			//overall result
			var simpleResult = new SimpleServerResult (TimeSpan.Zero, resultDictionary, request);

			Assert.IsTrue (simpleResult.Success, "The simple result did not report true when one of the client results where true");
		}

		[Test]
		public void SuccessfulAndFailedResultIsOveralSuccess () {
			//job
			var request = new MockJob ();

			//clients
			var client1 = new MockClient ();
			//params back are not important
			var clientResponseResults = new Dictionary<string, string> ();
			var result1 = new SimpleResult (true, clientResponseResults, request);

			var client2 = new MockClient ();
			//params back are not important
			var result2 = new SimpleResult (false, clientResponseResults, request); //this one fails!

			//set results
			var resultDictionary = new Dictionary<IClient, IResult> ();
			resultDictionary.Add (client1, result1);
			resultDictionary.Add (client2, result2);

			//overall result
			var simpleResult = new SimpleServerResult (TimeSpan.Zero, resultDictionary, request);

			Assert.IsTrue (simpleResult.Success, "The simple result did not report true when one of the client results where true");
		}

		[Test]
		public void SingleFailedResultIsOveralFailure () {
			//job
			var request = new MockJob ();

			//params back are not important
			var clientResponseResults = new Dictionary<string, string> ();

			//clients and results
			var client1 = new MockClient ();
			var result1 = new SimpleResult (false, clientResponseResults, request);

			//set results
			var resultDictionary = new Dictionary<IClient, IResult> ();
			resultDictionary.Add (client1, result1);

			//overall result
			var simpleResult = new SimpleServerResult (TimeSpan.Zero, resultDictionary, request);

			Assert.IsFalse (simpleResult.Success, "The simple result did not report false when all of the client results where false");
		}

		[Test]
		public void AllFailedResultIsOveralFailure () {
			//job
			var request = new MockJob ();

			//params back are not important
			var clientResponseResults = new Dictionary<string, string> ();

			var resultDictionary = new Dictionary<IClient, IResult> ();

			//clients and results
			//add multiple clients, all with success
			for (int i = 0; i < 10; i++) {
				//clients
				var client = new MockClient ();
				var result = new SimpleResult (false, clientResponseResults, request);

				//set results
				resultDictionary.Add (client, result);
			}

			//overall result
			var simpleResult = new SimpleServerResult (TimeSpan.Zero, resultDictionary, request);

			Assert.IsFalse (simpleResult.Success, "The simple result did not report true when one of the client results where true");
		}
	}
}

