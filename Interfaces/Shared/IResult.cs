using System;
using System.Collections.Generic;

namespace Interfaces.Shared
{
	public interface IResult
	{
		bool Success{ get; }

		/// <summary>
		/// Gets the result.
		/// This will be a list of the parameter names
		/// And the value associated with the param
		/// </summary>
		/// <value>The result.</value>
		IDictionary<string, string> Result { get; }

		IJob Request{ get; }
	}
}

