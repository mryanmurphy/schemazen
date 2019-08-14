using System;

namespace SchemaZen.Library.Models {
	[Flags]
	public enum CompareOptions {
		None = 0,
		IgnoreColumnPosition = 1,
	}
}
