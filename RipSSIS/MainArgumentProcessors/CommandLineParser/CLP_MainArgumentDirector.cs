using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

using CommandLine;
using static ExtensionMethods.UtilityFunctions;

namespace RipSSIS
{
    public class CLP_MainArgumentDirector : IMainArgumentDirector
    {
        static int returncode = -1; // -1 to indicate Not set.
        /// <summary>
        /// Program starts here, called either from Program.cs or RipSSISTests.MainArgumentProcessorTests.cs
        /// </summary>
        /// <param name="args">either real or simulated from test</param>
        /// <example>--batchmode true --packagepaths TestCase_00002.RealNoConn.Load_GL_Detail.dtsx --fullydetachfirst true</example>
        /// <returns>0 = success, 1 = fail</returns>
        public int DirectToProcessAccordingToArguments(string[] args)
        {
            returncode = 0; // Default to Pass

            // Capture assemblies loaded so far

            var assembliesLoadedBeforeSSIS = AppDomain.CurrentDomain.GetAssemblies();

            var res = Parser.Default.ParseArguments<CmdLnArgsAsProps>(args)
                .WithParsed<CmdLnArgsAsProps>(t => t.Execute(args))
                .WithNotParsed<CmdLnArgsAsProps>(err => HandleParseError(err));

            return returncode;
        }
         private static void HandleParseError(IEnumerable<Error> errs)
        {
            returncode = 1; // Fail (standard for command line.)
            IEnumerator<Error> enumerator = errs.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Print(enumerator.Current.Tag.ToString());// + " " + enumerator.Current.Tag.
                if (enumerator.Current is MissingRequiredOptionError)
                {
                    MissingRequiredOptionError mroErr = enumerator.Current as MissingRequiredOptionError;
                    Print("Option=" + mroErr.NameInfo.LongName);
                }
            }

            if (Debugger.IsAttached)
            {
                Console.ReadKey();
            }
        }
    }
}
