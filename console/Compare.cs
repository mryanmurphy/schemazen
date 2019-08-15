﻿using System;
using System.IO;
using ManyConsole;
using NDesk.Options;
using SchemaZen.Library.Command;

namespace SchemaZen.console {
	internal class Compare : ConsoleCommand {
		private string _source;
		private string _target;
		private string _outDiff;
		private bool _overwrite;
		private bool _verbose;
		private bool _ignoreColumnPosition;
		private bool _ignorePermissions;

		public Compare() {
			IsCommand("Compare", "CreateDiff two databases.");
			Options = new OptionSet();
			SkipsCommandSummaryBeforeRunning();
			HasRequiredOption(
				"s|source=",
				"Connection string to a database to compare.",
				o => _source = o);
			HasRequiredOption(
				"t|target=",
				"Connection string to a database to compare.",
				o => _target = o);
			HasOption(
				"outFile=",
				"Create a sql diff file in the specified path.",
				o => _outDiff = o);
			HasOption(
				"o|overwrite",
				"Overwrite existing target without prompt.",
				o => _overwrite = o != null);
			HasOption(
				"v|verbose",
				"Enable verbose mode (show detailed changes).",
				o => _verbose = o != null);
			HasOption(
				"ignoreColumnPosition",
				"Ignore column position when diffing.",
				o => _ignoreColumnPosition = o != null);
			HasOption(
				"ignorePermissions",
				"Ignore permissions when diffing.",
				o => _ignorePermissions = o != null);
		}

		public override int Run(string[] remainingArguments) {
			if (!string.IsNullOrEmpty(_outDiff)) {
				Console.WriteLine();
				if (!_overwrite && File.Exists(_outDiff)) {
					var question = $"{_outDiff} already exists - do you want to replace it";
					if (!ConsoleQuestion.AskYN(question)) {
						return 1;
					}
				}
			}

			var compareCommand = new CompareCommand {
				Source = _source,
				Target = _target,
				Verbose = _verbose,
				OutDiff = _outDiff,
				Overwrite = _overwrite,
				IgnoreColumnPosition = _ignoreColumnPosition,
				IgnorePermissions = _ignorePermissions,
			};

			try {
				return compareCommand.Execute() ? 1 : 0;
			} catch (Exception ex) {
				throw new ConsoleHelpAsException(ex.Message);
			}
		}
	}
}
