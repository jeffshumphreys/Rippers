using System;
using System.IO;
using System.Xml;
using ExtensionMethods;
using Microsoft.SqlServer.Dts.Runtime; // Dependent on C:\Program Files (x86)\Microsoft SQL Server\150\SDK\Assemblies\Microsoft.SQLServer.ManagedDTS.dll
using PackageExpansions;
using SQLExtensions;

namespace SSISExtensions
{
    public static class SSISExtensions
    {
        /// <summary>
        /// Load an SSIS dtsx package from the file system (not from SSIS system yet)
        /// Uses a trick (not found online) to first pull the file in as an XML Document, then alter it for offline mode, then move it from memory into a package object.
        /// The LoadPackage method from file to package will never load offline.
        /// </summary>
        /// <param name="package"></param>
        /// <param name="fullPathToPackage"></param>
        /// <returns>A properly fully populated package.</returns>

        public static Package LoadSSISPackageFileInOfflineMode(this Package package, string fullPathToPackage, PackageExplodedDetails packageExplodedDetails)
        {
            string PathPortion = Path.GetDirectoryName(fullPathToPackage);
            string FilePortion = Path.GetFileName(fullPathToPackage);

            XmlDocument SSISPackageAsXMLDoc = new XmlDocument();

            // Try to keep the processor from removing all new lines, which makes it readable.
            
            SSISPackageAsXMLDoc.PreserveWhitespace = true;

            // Loads the package file as XML rather than as an SSIS Package.

            SSISPackageAsXMLDoc.Load(fullPathToPackage);

            // There is at least one namespace necessary or our XPath references don't work: DTS

            XmlNamespaceManager nsmanager = new XmlNamespaceManager(SSISPackageAsXMLDoc.NameTable);
            nsmanager.AddNamespace("DTS", "www.microsoft.com/SqlServer/Dts");
            nsmanager.PushScope();

            // We'll use the DocumentElement from now on to reference the DOM.

            XmlElement root = SSISPackageAsXMLDoc.DocumentElement;

            // The very first node is Executable, and then it branches out from there

            XmlNode basenode = root.SelectSingleNode(@"/DTS:Executable", nsmanager);

            // There probably is a more efficient way to do this, but just always rebuilding the reference from root works.
            // I'm grabbing the PackageFormatVersion because this is a loose indicator (vague) of which TargetServerVersion we need to set in order to deal with Script Tasks.

            XmlNode PackageFormatVersionpropertynode = basenode.SelectSingleNode(@"/DTS:Executable/DTS:Property", nsmanager);
            int PackageFormatVersion                 = PackageFormatVersionpropertynode.InnerText.ToInt(valueifnull:-1);

            // An original piece of code that may not be necessary.
            // DelayValidation is a persistent property.

            basenode.SetAttributeOn(SSISPackageAsXMLDoc, "DTS:DelayValidation");
            basenode.SetAttributeOn(SSISPackageAsXMLDoc, "DTS:OfflineMode");

            // Tests show if these are not set then it will delay open.

            XmlNodeList connectionnodes = root.SelectNodes(@"/DTS:Executable/DTS:ConnectionManagers/DTS:ConnectionManager", nsmanager);

            foreach (XmlNode connectionnode in connectionnodes)
            {
                basenode.SetAttributeOn(SSISPackageAsXMLDoc, "DTS:DelayValidation");
                basenode.SetAttributeOn(SSISPackageAsXMLDoc, "DTS:OfflineMode");
            }

            // The project setting is the only way to really set offline status BEFORE establishing the package object-which is too late to make a difference in load time.

            Project temporaryInMemSSISProject = Project.CreateProject();

            // Only way to load a project in offline mode programmatically is to do these machinations.  OfflineMode can be set in a variety of ways, but it will flip to false every single time when saved or loaded from a file.

            temporaryInMemSSISProject.OfflineMode = true;

            // Set password here if needed or known.

            temporaryInMemSSISProject.Password = "";

            /*
                        SQL Version	Build #	PackageFormatVersion	Visual Studio Version
                        2005	    9		2   	                2005
                        2008	    10		3   	                2008
                        2008 R2	    10.5	3   	                2008
                        2012	    11		6   	                2010 or BI 2012
                        2014	    12		8   	                2012 CTP2 or 2013
                        2016	    13		8   	                2015/2017
                        2017	    14		8   	                2017
            */

            // For older software this must be set if you have Script Tasks.

            DTSTargetServerVersion forceXMLConversionToTargetServerVersion;

            switch (PackageFormatVersion)
            {
                case 2:
                case 3:
                case 6:
                    forceXMLConversionToTargetServerVersion = DTSTargetServerVersion.SQLServer2012;
                    break;
                case 8:
                    forceXMLConversionToTargetServerVersion = DTSTargetServerVersion.SQLServer2012; // What a mess.
                    break;
                default:
                    forceXMLConversionToTargetServerVersion = DTSTargetServerVersion.Latest;
                    break;
            }

            //temporaryInMemSSISProject.TargetServerVersion = forceXMLConversionToTargetServerVersion; // Hopefully this will help with Script Task loading.
            temporaryInMemSSISProject.TargetServerVersion = DTSTargetServerVersion.SQLServer2012;

            temporaryInMemSSISProject.PackageItems.Add(package, "tempPackageInMemory.dtsx");  // <= stream name doesn't matter.
            MyEventListener eventListener = new MyEventListener();
            RecordTestResults logger      = new RecordTestResults();

            // Log start in case it never comes back, or errors out.  And to time operation to success or errors out.

            int? createdrowkey = logger.RecordTestResultsToDb(
                        PathToTestPackage:            PathPortion
                      , PackageName:                  FilePortion
                      , AppVersion:                   packageExplodedDetails.ApplicationVersion
                      , PackageSize:                  packageExplodedDetails.PackageSizeBeforeLoad
                      , DateTimeStartedLoadFromXML:   DateTime.Now
                      , PackageFormatVersion:         PackageFormatVersion
                      , ProcessingStage:              "Immediately before we load from memory XML into the package"
                      );

            // This where the magic happens.  Massive 50+ MB packages can be loaded in seconds this way.

            try
            {
                package.LoadFromXML(SSISPackageAsXMLDoc.OuterXml, eventListener);  // 266 ms for 120KB, 10.3 seconds for 51MB.  Errors trying to extract ScriptTask for Display Connection Info.

                logger.RecordTestResultsToDb(
                            RecordToMerge: createdrowkey
                          , DateTimeExitededLoadFromXML: DateTime.Now
                          , ProcessingStage: "Immediately after we successfully loaded from memory XML into the package"
                          , Able_to_open_from_RipSSISPackage: true
                          );
            }
            catch
            {
                logger.RecordTestResultsToDb(
                            RecordToMerge: createdrowkey
                          , DateTimeExitededLoadFromXML: DateTime.Now
                          , ProcessingStage: "Immediately after we failed to load from memory XML into the package"
                          , Able_to_open_from_RipSSISPackage: false
                          );
                          throw;
            }

            return package;
    }

        class MyEventListener : DefaultEvents
        {
            public override bool OnError(DtsObject source, int errorCode, string subComponent,
              string description, string helpFile, int helpContext, string idofInterfaceWithError)
            {
                // Add application-specific diagnostics here.  
                Console.WriteLine("Error in {0}/{1} : {2}", source, subComponent, description);
                return false;
            }
        }
    }
}
