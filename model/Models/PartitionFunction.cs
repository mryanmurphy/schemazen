using System;
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

		public int InputParameterLength { get; set; }
		public byte InputParameterPrecision { get; set; }
		public int InputParameterScale { get; set; }

		public PartitionFunction(string name, string inputParameterType, RangeKind range) {
			Name = name;
			InputParameterType = inputParameterType;
			Range = range;
			BoundaryValues = new List<string>();
		}

		public PartitionFunction(string name, string inputParameterType, RangeKind range, int inputParameterLength, byte inputParameterPrecision, int inputParameterScale)
			: this (name, inputParameterType, range) {
			InputParameterLength = inputParameterLength;
			InputParameterPrecision = inputParameterPrecision;
			InputParameterScale = inputParameterScale;
		}

		public string ScriptCreate() {
			var text = new StringBuilder($"CREATE PARTITION FUNCTION {Name} (");

			switch (InputParameterType) {
				case "bigint":
				case "bit":
				case "date":
				case "datetime":
				case "datetime2":
				case "datetimeoffset":
				case "float":
				case "hierarchyid":
				case "int":
				case "money":
				case "real":
				case "smalldatetime":
				case "smallint":
				case "smallmoney":
				case "sql_variant":
				case "time":
				case "tinyint":
				case "uniqueidentifier":
				case "geography":
				case "sysname":
					text.Append(InputParameterType);
					break;

				case "binary":
				case "char":
				case "nchar":
				case "nvarchar":
				case "varbinary":
				case "varchar":
					var lengthString = InputParameterLength.ToString();
					if (lengthString == "-1") lengthString = "max";
					text.Append($"{InputParameterType}({lengthString})");
					break;

				case "decimal":
				case "numeric":
					text.Append($"{InputParameterType}({InputParameterPrecision},{InputParameterScale})");
					break;

				default:
					var message = $"Error scripting partition function {Name}."
						+ "SQL data type {InputParameterType} is not supported.";
					throw new NotSupportedException(message);
			}

			text.AppendLine(")");
			text.AppendLine($"AS RANGE {Range:G}");
			text.AppendLine($"FOR VALUES ({string.Join(", ", BoundaryValues.ToArray())});");
			return text.ToString();
		}
	}
}
