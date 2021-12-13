using System.Collections.Generic;

using CommandLine;

/*
 *  https://dotnetfiddle.net/wrcAxr
 *  https://stackoverflow.com/questions/54672567/parser-default-parsearguments-always-returns-false
 *  https://www.nuget.org/packages/CommandLineParser/1.9.71
 *  https://github.com/commandlineparser/commandline
 */
namespace RipSSIS
{
    public class CommandLineArgumentsAsProperties : ICommand
    {
        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }

        [Option('p', "packagepaths", Required = true, HelpText = "Path(s) to SSIS package to decompile, reverse engineer, expand, explode, deconstruct, disassemble. Use ; to separate.", Separator = ';')]
        public IEnumerable<string> PackagePaths
        { get; set; }

        [Option('b', "batchmode", Required = false, Default =false, HelpText = "Set if you want to prevent any blocking operations. Needed for Unit Testing, too. Otherwise it stops on a getchar.")]
        public bool BatchMode { get; set; }

        [Option('k', "skipfileverify", Required = false, Default = false, HelpText = "Set if you want to prevent the File.Exists check, and other checks made.")]
        public bool SkipFileVerifications { get; set; }

        [Option('w', "wraplinesonscreen", Required = false, Default = false, HelpText = "Wrap the lines on the screen at 80, but doesn't break on words, so don't use.")]
        public bool WrapLinesOnScreen { get; set; }

        [Option(longName: "skipoldstuff", Required = false, Default = false, HelpText = "For testing all sorts of packages, I've hardcoded some skips on SQL or scripts, etc., that I don't want to break on and reanalyze.")]
        public bool SkipOldStuff { get; set; }

        [Option('s', "savetofile", Required = false, Default = null, HelpText = @"Set a file path to write out to. use %USERPROFILE%\Documents\ and macro {{PACKAGENAMENOEXTENSION}} to just copy the package name.")]
        public string SaveResultsToFile { get; set; }

        public void Execute(string[] commandLineArgs)
        {
            var processHandler = new ProcessUserRequest(this, commandLineArgs);
        }
    }
}
