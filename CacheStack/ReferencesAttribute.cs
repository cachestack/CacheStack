using System;

namespace CacheStack
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Struct)]
	public class ReferencesAttribute : Attribute
	{
		public Type Type { get; set; }

		public ReferencesAttribute(Type type)
		{
			Type = type;
		}
	}
}
