using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using Xunit;
using System.Diagnostics;

namespace RipSSIS.Tests
{
    // https://github.com/commandlineparser/commandline/wiki/CommandLine-Grammar
    // Called from CLP_MainArgumentDirector.cs
    public class MainArgumentProcessorTests
    {
        const string ARG_PACKAGEPATHS = "--packagepaths";
        const string ARG_FULLYDETACHFIRST = "--fullydetachfirst";
        const string TRUE = "true";
        const string basepath = @"D:\Rippers\RipSSISTestCases\"; // Bad path!
        List<string> args = new List<string>(new string[] { "--batchmode", TRUE });
        public MainArgumentProcessorTests()
        {

        }

        [Fact()]
        public void WhenRunningProgram_ShouldFailIfNoPathArg()
        {
            var commandlineargs = string.Join(" ", args);
            Debug.WriteLine(commandlineargs);
            int rtnval = new CLP_MainArgumentDirector().DirectToProcessAccordingToArguments(args.ToArray());
            Assert.True(rtnval == 1, "Correctly returned failure code when no path given.");
        }

        [Fact]
        public void WhenRunningProgram_ShouldFailOnBadPackagePath()
        {
            args.AddRange(new[] { ARG_PACKAGEPATHS, @"D:\xxxxx.xxxx" });
            var commandlineargs = string.Join(" ", args);
            Debug.WriteLine(commandlineargs);
            int rtnval = -1;
            Action act = () => rtnval = new CLP_MainArgumentDirector().DirectToProcessAccordingToArguments(args.ToArray());

            Exception exception = Assert.Throws<Exception>(act);
            //Assert.Equal("Package not found: " + fullPathToPackage", exception.Message)
        }

        [Fact]
        public void WhenRunningProgram_ShouldPassOnKnownGoodPackagePath()
        {
            const string packagepath = basepath + "TestCase_00001.ExecSQL_TRUNCATE.dtsx";
            Assert.True(File.Exists(packagepath));
            args.AddRange(new[] { ARG_PACKAGEPATHS, packagepath });
            var commandlineargs = string.Join(" ", args);
            Debug.WriteLine(commandlineargs);
            int rtnval = new CLP_MainArgumentDirector().DirectToProcessAccordingToArguments(args.ToArray());
            Assert.True(rtnval == 0, "Correctly processed the test package that is a known good package.");
        }

        [Fact]
        public void WhenRunningProgram_ShouldOpenAsXMLAndInject()
        {
            const string packagepath = basepath + "TestCase_00001.ExecSQL_TRUNCATE.dtsx";
            Assert.True(File.Exists(packagepath));
            args.AddRange(new[] { ARG_PACKAGEPATHS, packagepath });
            args.AddRange(new[] { ARG_FULLYDETACHFIRST, TRUE });
            var commandlineargs = string.Join(" ", args);
            Debug.WriteLine(commandlineargs);
            int rtnval = new CLP_MainArgumentDirector().DirectToProcessAccordingToArguments(args.ToArray());
            Assert.True(rtnval == 0, "Correctly opened SSIS as XML, injected flags, and saved back out to new name.");
        }
        // Verify output is generated.
        // Verify strings present.
        // Test multiple package paths in.
        // Test duplicate package paths in.
    }
}