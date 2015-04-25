using System;

namespace Interfaces.Shared
{
	public interface IClient
	{
		string Name {
			get;
			set;
		}

		int Id {
			get;
		}

		IResult RunJob (IJob job);
	}
}

