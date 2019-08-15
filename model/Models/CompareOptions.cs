using System;

namespace SchemaZen.Library.Models {

	[Flags]
	public enum CompareOptions {
		None = 0,
		IgnoreColumnPosition = 1 << 1,
		IgnorePermissions = 1 << 2,
	}

}
