using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

using ExtensionMethods;

using Microsoft.SqlServer.Dts.Runtime;

using PackageExpansions;

using SSISExtensions;

using Xunit;

using static ExtensionMethods.UtilityFunctions;
using static ExtensionMethods.SharedConstants;
using static ExtensionMethods.CollectionExtensions;
using static ExtensionMethods.MiscExtensions;
using static ExtensionMethods.SMOExtensions;
using static ExtensionMethods.ExtendSqlDataReader;

// SSIS

using DtsRuntime = Microsoft.SqlServer.Dts.Runtime;

namespace RipSSIS
{
    /// <summary>
    /// Handles initial processing and loading of packages, including converting CLI arguments into paths and/or specific files.
    /// </summary>
    public class ProcessUserRequest
    {
        /// <summary>
        /// Prevent someone creating an uninstantiated object, which could result in serious breakage.
        /// </summary>
        private ProcessUserRequest() {}

        /// <summary>
        /// Process either command line arguments or simulated arguments from RipSSISTests package.
        /// </summary>
        /// <param name="cliArguments">Interpreted command line arguments into a class</param>
        public ProcessUserRequest(CmdLnArgsAsProps cliArguments, string[] commandLineArgs)
        {
            string divinedBasePathFromMachine;
            int sessionID = Environment.CurrentManagedThreadId;

            // Determine where our example packages will be.

            switch (Environment.MachineName)
            {
                case HOME_MACHINE:
                    divinedBasePathFromMachine = HOME_PATH_TO_EXAMPLE_PACKAGES;
                    break;

                case CONTRACTOR_MACHINE:
                    divinedBasePathFromMachine = CONTRACTOR_PATH_TO_EXAMPLE_PACKAGES;
                    break;

                default:
                    throw new Exception($"Don't recognize your machine {Environment.MachineName}. Only recognize {HOME_MACHINE} or {CONTRACTOR_MACHINE}");
            }

            // No obvious dups. There can be dups other ways, we check that further down.

            if (cliArguments.PackagePaths.ToList().Count != cliArguments.PackagePaths.Distinct().Count())
            {
                throw new Exception("Duplicated paths in input set, which could cause errors.");
            }

            // Split out each filepath and build up partials (names) into actual paths, and verify each one exists before starting processing, since it's a batch process we don't want to fail overnight.

            List<CapturedPackageDetailsBeforeLoading> expandedFilePaths = new List<CapturedPackageDetailsBeforeLoading>(50);

            foreach (var fileIdentifierGivenByUser in cliArguments.PackagePaths.ToList<string>())
            {
                ConstructSinglePath(fileIdentifierGivenByUser);

                void ConstructSinglePath(string lfileIdentifierGivenByUser)
                {
                    Assert.False(lfileIdentifierGivenByUser.Trim() == String.Empty);

                    // Is this a folder or a file?

                    string possiblefolder = lfileIdentifierGivenByUser;
                    bool? processAsFolder = null;

                    if (Directory.Exists(possiblefolder))
                    {
                        processAsFolder = true;
                    }
                    else if (!Directory.Exists(possiblefolder))
                    {
                        // Doesn't work: possiblefolder = Path.Combine(divinedBasePathFromMachine, possiblefolder);
                        string extendedpossiblefolder = CombineFolders(divinedBasePathFromMachine, possiblefolder);

                        if (Directory.Exists(extendedpossiblefolder))
                        {
                            possiblefolder = extendedpossiblefolder;
                            processAsFolder = true;
                        }
                        else if (divinedBasePathFromMachine.EndsWith(possiblefolder))
                        {
                            possiblefolder = divinedBasePathFromMachine;
                            processAsFolder = true;
                        }
                        else
                        {
                            processAsFolder = false;
                        }
                    }

                    if (processAsFolder == true)
                    {
                        // Expand out into all individual packages in folder.  Don't recurse down yet.

                        foreach (var packagedfile in Directory.GetFiles(possiblefolder, "*.dtsx", SearchOption.TopDirectoryOnly))
                        {
                            ConstructSinglePath(packagedfile);
                        }
                    }

                    if (processAsFolder == false)
                    {
                        CapturedPackageDetailsBeforeLoading                           capturedPackageDetailsBeforeLoading = new CapturedPackageDetailsBeforeLoading();
                        capturedPackageDetailsBeforeLoading.fileIdentifierGivenByUser                                     = lfileIdentifierGivenByUser;

                        // Build this one out with full path and extension.

                        string fullPathToPackageFile = lfileIdentifierGivenByUser;

                        // Does it have a drive letter at the beginning, or a relative path?

                        if (lfileIdentifierGivenByUser.Like(@"^[a-zA-Z]\:") != true && lfileIdentifierGivenByUser.Like("^//") != true)
                        {
                            fullPathToPackageFile = Path.Combine(divinedBasePathFromMachine, fullPathToPackageFile);
                        }


                        if (!lfileIdentifierGivenByUser.EndsWith(".dtsx")) fullPathToPackageFile += ".dtsx";

                        if (!File.Exists(fullPathToPackageFile))
                        {
                            throw new Exception("Package not found at path: " + fullPathToPackageFile + (fullPathToPackageFile != lfileIdentifierGivenByUser ? ", input identifier was " 
                                + lfileIdentifierGivenByUser + "." : "input identifer was the same."));
                        }

                        capturedPackageDetailsBeforeLoading.expandedPath = fullPathToPackageFile;

                        // Snapshot file details.

                        FileInfo                                                fileInfo          = new FileInfo(fullPathToPackageFile);
                        capturedPackageDetailsBeforeLoading.originalFileSize                      = fileInfo.Length;
                        capturedPackageDetailsBeforeLoading.created                               = fileInfo.CreationTime;
                        capturedPackageDetailsBeforeLoading.originalLastWritten                   = fileInfo.LastWriteTime;
                        byte[]                                                  packageBytes      = File.ReadAllBytes(fullPathToPackageFile);
                        byte[]                                                  packageFileMDHash = new MD5CryptoServiceProvider().ComputeHash(packageBytes);
                        capturedPackageDetailsBeforeLoading.originalFileMD5Hash                   = packageFileMDHash;

                        expandedFilePaths.Add(capturedPackageDetailsBeforeLoading);
                    }
                }
            }

            // Now that we've expanded all partial paths to files or folders, and verified existence and verified no dups, now we run through the list.

            if (expandedFilePaths.Count != expandedFilePaths.Distinct().Count())
            {
                throw new Exception("Duplicated paths in input set after expanding, which could cause errors.");
            }

            foreach (var expandedFilePathDetail in expandedFilePaths)
            {
                // Start building up all our attributes into one class instance.

                PackageExplodedDetails                   packageExplodedDetails = new PackageExplodedDetails(expandedFilePathDetail);
                
                packageExplodedDetails.SaveResultsToFile                        = cliArguments.SaveResultsToFile;
                packageExplodedDetails.SkipFileVerifications                    = cliArguments.SkipFileVerifications;
                packageExplodedDetails.BatchMode                                = cliArguments.BatchMode;
                packageExplodedDetails.SkipOldStuff                             = cliArguments.SkipOldStuff; // For speeding up seeking for variations.
                packageExplodedDetails.WrapLinesOnScreen                        = cliArguments.WrapLinesOnScreen; 
                packageExplodedDetails.CommandLineArgs                          = commandLineArgs;

                // Now disassemble the package

                RipSSISPackage(expandedFilePathDetail.expandedPath, packageExplodedDetails);
            }
        }

        /// <summary>
        /// Called with the finalized full path to a singular DTSX, and any captured information about it, in case we want to catch in long runs if the source file has changed.
        /// </summary>
        /// <param name="fullPathToPackage"></param>
        /// <param name="packageExplodedDetails"></param>
        static public void RipSSISPackage(string fullPathToPackage, PackageExplodedDetails packageExplodedDetails)
        {
            // Not sure this is necessary, but set our global outputter to screen.  Globals will probably not work if we go parallel, but then parallel processes writing to the console may be messy.

            PrintToScreen(true);

            // This currently the way the user notifies that they want a file capture, by setting a path. A flag in addition would be good so they don't have to delete the save path in their command line.

            if (packageExplodedDetails.SaveResultsToFile != null)
            {
                PrintToFile(packageExplodedDetails.SaveResultsToFile);
            }

            Print("Loading package " + fullPathToPackage + " from file system into in-memory package object.");
            Package package = new Package().LoadSSISPackageFileInOfflineMode(fullPathToPackage, packageExplodedDetails);

            Assert.NotNull(package);

            // Grab all the interesting properties, and only if they have non-default or non-empty or non-null properties, and (for now) print them out.

            var masterExpressedProperties = new ExpressAllInterestingPropsOfClass(package);
            masterExpressedProperties.ListOut();

            if (package.Configurations.Count > 0)
            {
                Print("Package Configurations");
                Indent();
                foreach (var packageConfiguration in package.Configurations)
                {
                    Print($"Configuration Name: {packageConfiguration.Name}, Configuration String: {packageConfiguration.ConfigurationString}, {packageConfiguration.ConfigurationType.ToString()}");
                }
                UnIndent();
            }
            else
            {
                Print("No Package Configurations found.");
            }
            
            if (package.Parameters.Count > 0)
            {
                Print("Package Parameters");
                Indent();
                foreach (var packageParameter in package.Parameters)
                {
                    Print($"Package Parameter: {packageParameter.ToString()}");//, {packageParameter.");
                }
                UnIndent();
            }
            else
            {
                Print("No Package Parameters found.");
            }
            if (package.Connections.Count > 0)
            {
                Print("Package Connections");
                Indent();
                foreach (var packageConnection in package.Connections)
                {
                    //ExpressAllInterestingPropsOfClass propsOfClass = new ExpressAllInterestingPropsOfClass(packageConnection);
                    //propsOfClass.ListOut();
                }
                UnIndent();
            }
            else
            {
                Print("No Package Connections found.");
            }

            if (package.PrecedenceConstraints.Count > 0)
            {
                foreach (var precedenceConstraint in package.PrecedenceConstraints)
                {
                    Print("Precedence Constraint: " + precedenceConstraint.Name);
                    Indent();
                   // Print("Previous Executable Task: " + ((TaskHost)(precedenceConstraint.PrecedenceExecutable)).InnerObject.ToString() );
                   // Print("Next Executable Task: " + ((TaskHost)(precedenceConstraint.ConstrainedExecutable)).InnerObject.ToString() );
                    //if (precedenceConstraint.Expression != "") Print("Expression: " + precedenceConstraint.Expression);

                    //new PackageExpander(package).InterpretPackageComponent(precedenceConstraint.PrecedenceExecutable, "PrecedenceConstraintLayer");
                    UnIndent();
                }
            }
            else
            {
                Print("No Precedence Constraints (The little lines) found.");
            }
            if (package.LogProviders.Count > 0)
            {
                foreach (var logProvider in package.LogProviders)
                {
                    Print("Log Provider: " + logProvider.Name);
                }
            }

            //ExpressAllInterestingPropsOfClass

            foreach (DtsRuntime.Executable packageExecutable in package.Executables)
            {
                new PackageExpander(package, packageExplodedDetails).InterpretPackageComponent(packageExecutable, layer: "TopPackageLayer");
            }

            Console.WriteLine("Completed package expansion.");

            if (printtofile)
            {
                SavePrintsToFile();
                Print ($"File saved to {filepathtoprintto}", true, nowrap: true);
            }

            if (Environment.UserInteractive && !Console.IsInputRedirected)
            {
                if (!packageExplodedDetails.BatchMode)
                {
                    Print();
                    Print("Press any key to close screen...");
                    var c = Console.ReadKey().KeyChar;
                }
            }

        }
    }
}
