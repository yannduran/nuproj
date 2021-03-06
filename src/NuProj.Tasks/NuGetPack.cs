﻿using System;
using System.IO;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace NuProj.Tasks
{
    public class NuGetPack : ToolTask
    {
        [Required]
        public string NuSpecPath { get; set; }

        public string OutputDirectory { get; set; }

        public bool Symbols { get; set; }

        public bool NoPackageAnalysis { get; set; }

        public bool NoDefaultExcludes { get; set; }

        public bool ExcludeEmptyDirectories { get; set; }

        protected override string ToolName
        {
            get { return "NuGet -pack"; }
        }

        protected override string GenerateFullPathToTool()
        {
            return Path.Combine(ToolPath, ToolExe);
        }

        protected override string GenerateCommandLineCommands()
        {
            var builder = new CommandLineBuilder();
            builder.AppendSwitch("pack");
            builder.AppendFileNameIfNotNull(NuSpecPath);
            builder.AppendSwitchIfNotNull("-OutputDirectory ", OutputDirectory);

            if (Symbols)
                builder.AppendSwitch("-Symbols");

            if (NoPackageAnalysis)
                builder.AppendSwitch("-NoPackageAnalysis");

            if (NoDefaultExcludes)
                builder.AppendSwitch("-NoDefaultExcludes");

            if (ExcludeEmptyDirectories)
                builder.AppendSwitch("-ExcludeEmptyDirectories");

            return builder.ToString();
        }

        protected override MessageImportance StandardErrorLoggingImportance
        {
            get { return MessageImportance.High; }
        }

        protected override void LogEventsFromTextOutput(string singleLine, MessageImportance messageImportance)
        {
            if (messageImportance == MessageImportance.High)
            {
                Log.LogError(singleLine);
                return;
            }

            if (singleLine.StartsWith("Issue:") || singleLine.StartsWith("Description:") || singleLine.StartsWith("Solution:"))
            {
                Log.LogWarning(singleLine);
                return;
            }

            base.LogEventsFromTextOutput(singleLine, messageImportance);
        }
    }
}