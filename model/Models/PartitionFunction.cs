using System.Collections.Generic;
using System.Text;

namespace SchemaZen.Library.Models {

	public class PartitionFunction : INameable, IScriptable {

		public enum RangeKind {
			LEFT,
			RIGHT,
		}

		// https://dba.stackexchange.com/questions/171365/how-to-generate-the-creation-script-for-partition-function-and-partition-schema

		/// <summary>
		/// Is the name of the partition function. Partition function names must be unique within the database and comply with the rules for identifiers.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Is the data type of the column used for partitioning.
		/// All data types are valid for use as partitioning columns,
		/// except text, ntext, image, xml, timestamp, varchar(max), nvarchar(max),
		/// varbinary(max), alias data types, or CLR user-defined data types.
		/// </summary>
		public string InputParameterType { get; set; }

		/// <summary>
		/// Specifies to which side of each boundary value interval, left or right, the boundary_value [ ,...n ] belongs,
		/// when interval values are sorted by the Database Engine in ascending order from left to right.<para></para>
		/// If not specified, LEFT is the default.
		/// </summary>
		public RangeKind Range { get; set; }

		/// <summary>
		/// Specifies the boundary values for each partition of a partitioned table or index that uses partition_function_name. <para></para>
		/// If boundary_value is empty, the partition function maps the whole table or index using partition_function_name into a single partition. <para></para>
		/// Only one partitioning column, specified in a CREATE TABLE or CREATE INDEX statement, can be used.
		/// </summary>
		public List<string> BoundaryValues { get; }

		public PartitionFunction(string name, string inputParameterType, IEnumerable<string> boundaryValues, RangeKind range = RangeKind.LEFT) {
			Name = name;
			InputParameterType = inputParameterType;
			BoundaryValues = new List<string>(boundaryValues);
			Range = range;
		}

		public string ScriptCreate() {
			var text = new StringBuilder($"CREATE PARTITION FUNCTION {Name} ({InputParameterType})");
			text.AppendLine($"AS RANGE {Range:G}");
			text.AppendLine($"FOR VALUES ({string.Join(",", BoundaryValues.ToArray())});");
			return text.ToString();
		}
	}
}
