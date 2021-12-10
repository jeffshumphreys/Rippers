using System.Deployment.Application;
using System.Reflection;
using System.IO;
using System;
using ExtensionMethods;

namespace PackageExpansions
{
    /// <summary>
    /// Consolidate package details from multiple sources into a data packet aka bean to carry data around
    /// Try not to over add attributes and methods here.  Avoid methods especially if they have business logic (!)
    /// Sources include FileInfo, command line run through the command line parser, the command line actual strings, environment variables.
    /// </summary>
    public class PackageExplodedDetails
    {
        public PackageExplodedDetails(CapturedPackageDetailsBeforeLoading capturedPackageDetailsBeforeLoading)
        {
            ApplicationVersion                       = ApplicationDeployment.IsNetworkDeployed ? ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString() : Assembly.GetExecutingAssembly().GetName().Version.ToString();
            ProcessId                                = System.Diagnostics.Process.GetCurrentProcess().Id;
            FullPackageFileName                      = capturedPackageDetailsBeforeLoading.expandedPath;
            PackageFileNameWithExtension             = Path.GetFileName(capturedPackageDetailsBeforeLoading.expandedPath);
            PackageFileNameNoExtension               = Path.GetFileNameWithoutExtension(capturedPackageDetailsBeforeLoading.expandedPath);
            this.CapturedPackageDetailsBeforeLoading = capturedPackageDetailsBeforeLoading;
        }

        public string PackageFileNameNoExtension { get; set; }
        public string FullPackageFileName { get; private set; }
        public string PackageFileNameWithExtension { get; }
        public string ApplicationVersion { get; set; }
        public long PackageSizeBeforeLoad { get; set; }
        public int ProcessId { get; set; }
        public bool BatchMode { get; set; }
        public bool SkipFileVerifications { get; set; }
        public bool SkipOldStuff { get; set; }
        public bool WrapLinesOnScreen { get; set; }
        public string[] CommandLineArgs { get; set; }
        CapturedPackageDetailsBeforeLoading capturedPackageDetailsBeforeLoading;
        public CapturedPackageDetailsBeforeLoading CapturedPackageDetailsBeforeLoading { get => capturedPackageDetailsBeforeLoading; set => capturedPackageDetailsBeforeLoading = value; }

        private string saveResultsToFile;

        public string SaveResultsToFile
        {
            get { return saveResultsToFile; }
            set {
                if (value == null)
                {// User did not pass in a save path.
                    saveResultsToFile = value;
                    return;
                }

                saveResultsToFile = value.Replace("{{PACKAGENAMENOEXTENSION}}", PackageFileNameNoExtension);

                saveResultsToFile = saveResultsToFile.Replace("{{DATETIME}}", DateTime.Now.ToString("yyyyMMddhhmmssFFF"));  
                string userProfile = Environment.GetEnvironmentVariable("USERPROFILE");
                saveResultsToFile = saveResultsToFile.Replace("%USERPROFILE%", userProfile +"\\");
                saveResultsToFile = saveResultsToFile.Replace("\\\\", "\\");

                if (saveResultsToFile.Contains("{{"))
                {
                    throw new ArgumentException("Macros still embedded in save file path");
                }
            }
        }

    }
}
