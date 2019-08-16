using System.Collections.Generic;
using System.Text;

namespace SchemaZen.Library.Models
{
	public class PartitionScheme : INameable, IScriptable
	{
		// https://dba.stackexchange.com/questions/171365/how-to-generate-the-creation-script-for-partition-function-and-partition-schema

		/// <summary>
		/// Is the name of the partition scheme. Partition scheme names must be unique within the database and comply with the rules for identifiers.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The <see cref="PartitionFunction"/> being referenced by this <see cref="PartitionScheme"/> <para></para>
		/// Partitions created by the partition function are mapped to the filegroups specified in the partition scheme.
		/// partition_function_name must already exist in the database.
		/// A single partition cannot contain both FILESTREAM and non-FILESTREAM filegroups.
		/// </summary>
		public string PartitionFunctionName { get; set; }

		/// <summary>
		/// Specifies that all partitions map to the filegroup provided in file_group_name, or to the primary filegroup if [PRIMARY] is specified. If ALL is specified, only one file_group_name can be specified.
		/// </summary>
		public bool IsAllSpecified { get; set; }

		/// <summary>
		/// Specifies the names of the filegroups to hold the partitions specified by partition_function_name. file_group_name must already exist in the database.
		/// </summary>
		public List<string> FileGroupNames { get; }

		public PartitionScheme(string name, string partitionFunctionName, bool isAllSpecified, IEnumerable<string> fileGroupNames) {
			Name = name;
			PartitionFunctionName = partitionFunctionName;
			IsAllSpecified = isAllSpecified;
			FileGroupNames = new List<string>(fileGroupNames);
		}

		public string ScriptCreate()
		{
			var text = new StringBuilder($"CREATE PARTITION SCHEME {Name}");
			text.AppendLine($"AS PARTITION {PartitionFunctionName}");

			if (IsAllSpecified) {
				text.AppendLine($"ALL TO ({FileGroupNames[0]})");
			}
			else {
				text.AppendLine($"TO ({string.Join(", ", FileGroupNames.ToArray())});");
			}

			return text.ToString();
		}
	}
}
