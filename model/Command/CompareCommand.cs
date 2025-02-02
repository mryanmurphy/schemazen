using System;
using System.IO;
using SchemaZen.Library.Models;

namespace SchemaZen.Library.Command {
	public class CompareCommand : BaseCommand {
		public string Source { get; set; }
		public string Target { get; set; }
		public bool Verbose { get; set; }
		public string OutDiff { get; set; }
		public bool IgnoreColumnPosition { get; set; }
		public bool IgnorePermissions { get; set; }
		public bool IgnoreRoutines { get; set; }

		public CompareOptions CompareOptions {
			get {
				var flags = CompareOptions.None;

				if (IgnoreColumnPosition) flags |= CompareOptions.IgnoreColumnPosition;
				if (IgnorePermissions) flags |= CompareOptions.IgnorePermissions;
				if (IgnoreRoutines) flags |= CompareOptions.IgnoreRoutines;

				return flags;
			}
		}

		public bool Execute() {
			var sourceDb = new Database();
			var targetDb = new Database();
			sourceDb.Connection = Source;
			targetDb.Connection = Target;
			sourceDb.Load();
			targetDb.Load();
			var diff = sourceDb.Compare(targetDb, CompareOptions);
			if (diff.IsDiff) {
				Console.WriteLine("Databases are different.");
				Console.WriteLine(diff.SummarizeChanges(Verbose));
				if (!string.IsNullOrEmpty(OutDiff)) {
					Console.WriteLine();
					if (!Overwrite && File.Exists(OutDiff)) {
						var message =
							$"{OutDiff} already exists - set overwrite to true if you want to delete it";
						throw new InvalidOperationException(message);
					}

					File.WriteAllText(OutDiff, diff.Script());
					Console.WriteLine(
						$"Script to make the databases identical has been created at {Path.GetFullPath(OutDiff)}");
				}

				return true;
			}

			Console.WriteLine("Databases are identical.");
			return false;
		}
	}
}
