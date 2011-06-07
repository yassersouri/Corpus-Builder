using System;

namespace DependencyBasedSentenceAnalyzer
{
	[Flags]
	public enum NumberType
	{
		INVALID = 0,
		SINGULAR = 1,
		PLURAL = 2
	}
}
