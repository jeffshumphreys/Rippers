using System;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using System.Net;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

// SSIS
using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;

// T-SQL

using Microsoft.SqlServer.TransactSql.ScriptDom;
using Microsoft.SqlServer.Dac.Model;

using SSISExtensions;
using PackageExpansions;
using ExtensionMethods;
using static ExtensionMethods.SharedConstants;
using static ExtensionMethods.UtilityFunctions;
using Microsoft.SqlServer.Dts.Tasks.WebServiceTask;

namespace RipSSIS
{
    /// <summary>
    /// Actual work of traversing the package Component tree is isolated to keep simpler.
    /// </summary>
    public class PackageExpander
    {
        public Package package { get; set; }
        public PackageExplodedDetails PackageExplodedDetailsInstance { get; }

        public PackageExpander(Package package, PackageExplodedDetails packageExplodedDetails)
        {
            this.package = package;
            PackageExplodedDetailsInstance = packageExplodedDetails;
        }

        /// <summary>
        /// Recursive function.  Steps down hierarchy of tasks, sequences, etc.
        /// </summary>
        /// <param name="packageComponent"></param>
        /// <param name="currentLevel"></param>
        public void InterpretPackageComponent(object packageComponent, string layer /*precedence constraint, or package body*/, int currentLevel = 1, string taskHostName = null)
        {
            switch (packageComponent)
            {
                //--------------------------------------------------------------------------------------------------------------------------------------------------------
                case Microsoft.SqlServer.Dts.Pipeline.Wrapper.MainPipe mainPipe:
                    Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSComponentMetaDataCollection100 componentMetaDataCollection = mainPipe.ComponentMetaDataCollection;

                    var pipeComponentCount = componentMetaDataCollection.Count; // Possibly always 2.

                    IDTSPathCollection100 pathCollection = mainPipe.PathCollection;
                    
                    foreach (var pathObject in pathCollection)
                    {
                        IDTSPath100 pathItem = pathObject as IDTSPath100;

                        Print($"IDTSPath100 Identification: {pathItem.IdentificationString}");
                        Print($"IDTSPath100 Name: {pathItem.Name}");

                        IDTSInput100 endpoint = pathItem.EndPoint;
                        IDTSOutput100 startpoint = pathItem.StartPoint;
                        Indent();
                        Print($"Path Start Point: {startpoint.IdentificationString}");
                        Print($"Path End Point: {endpoint.IdentificationString}");
                        UnIndent();
                    }

                    Indent();

                    // Data Flows

                    foreach (Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSComponentMetaData100 componentMetaData in componentMetaDataCollection) // Again, two component metadata items.
                    {
                        if (PackageExplodedDetailsInstance.SkipOldStuff && componentMetaData.IdentificationString.In("F4715I Error Destination 1", "F4715I Source", "AB_Warehouse_Changes", "TF SalesForce Destination")) continue;
                        var componentMetaDataName = componentMetaData.Name;
                        Print($"Component: {componentMetaDataName}");
                        string contactInfo = componentMetaData.ContactInfo;
                        var componentView = componentMetaData.GetComponentView();

                        var runtimeCollection = componentMetaData.RuntimeConnectionCollection;

                        foreach (IDTSRuntimeComponent100 runtimeComponent in runtimeCollection)
                        {
                            //runtimeComponent.
                        }

                        //An unhandled exception of type 'System.Runtime.InteropServices.COMException' occurred in RipSSIS.exe
                        //var componentMetaDataGetComponentView = componentMetaData.GetComponentView();
                        IDTSInputCollection100 componentMetaDataInputCollection = componentMetaData.InputCollection;
                        
                        foreach (Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSInput100 input in componentMetaDataInputCollection)
                        {
                            IDTSCustomPropertyCollection100         customPropertyCollection         = input.CustomPropertyCollection;
                            foreach (IDTSCustomProperty100 customProperty in customPropertyCollection)
                            {
                                Print($"Custom Property: {customProperty.Name}");
                            }

                            IDTSExternalMetadataColumnCollection100 externalMetadataColumnCollection = input.ExternalMetadataColumnCollection;
                            var isused = externalMetadataColumnCollection.IsUsed;

                            foreach (IDTSExternalMetadataColumn100 externalMetadataColumn in externalMetadataColumnCollection)
                            {
                                Print($"External Metadata Column: {externalMetadataColumn.ID}");                                // 25
                                Print($"External Metadata Column: {externalMetadataColumn.Name}");                              // AddressID
                                Print($"External Metadata Mapped Column: {externalMetadataColumn.MappedColumnID}");             // 0
                                Print($"External Metadata Column Data Type: {externalMetadataColumn.DataType}");                // DT_I4 (3)
                                Print($"External Metadata Column Data Type: {externalMetadataColumn.ObjectType}");              // OT_EXTERNALMETADATACOLUMN (16384)
                                Print($"External Metadata Column Description: {externalMetadataColumn.Description}");
                                Print($"External Metadata Column Description: {externalMetadataColumn.Length}");                // 0
                                Print($"External Metadata Column Description: {externalMetadataColumn.IdentificationString}");  // "OLE DB Destination.Inputs[OLE DB Destination Input].ExternalColumns[AddressID]"

                                // 26
                                // AddressLine1
                                // 0
                                // DT_WSTR (130)
                                // OT_EXTERNALMETADATACOLUMN (16384)

                                // 60
                                // "OLE DB Destination.Inputs[OLE DB Destination Input].ExternalColumns[AddressLine1]"

                                // DT_IMAGE (301), DT_GUID (72)
                                // Microsoft.SqlServer.Dts.Runtime.Wrapper.DataType.DT_DBTIMESTAMP 135
                            }

                            IDTSInputColumnCollection100 inputColumnCollection = input.InputColumnCollection;

                            foreach (IDTSInputColumn130 inputColumn in inputColumnCollection)
                            {
                                Print($"Input Column: {inputColumn.Name}");
                                Print($"Input Column Is Valid?: {inputColumn.IsValid}");                                    
                                Print($"Input Column Identification: {inputColumn.IdentificationString}");                  // "OLE DB Destination.Inputs[OLE DB Destination Input].Columns[AddressID]"
                                Print($"Input Column Lineage Identification: {inputColumn.LineageIdentificationString}");   // "OLE DB Source.Outputs[OLE DB Source Output].Columns[AddressID]"
                                Print($"Input Column Lineage ID: {inputColumn.LineageID}");
                                Print($"Input Column Is Associated with an Output Column?: {inputColumn.IsAssociatedWithOutputColumn}");
                                Print($"Input Column ID: {inputColumn.ID}");
                                Print($"Input Upstream Component: {inputColumn.UpstreamComponentName}");                    // "OLE DB Source"
                            }
                            // OOPS! this is identical to componentMetaData:
                            // IDTSComponentMetaData100 InputComponent = input.Component;
                            
                            // Interesting: input.GetVirtualInput

                            switch (input)
                            {
                                case IDTSInput100 inputcast:
                                    Print($"Data Flow object: ${inputcast.Name}");
                                    Print($"Data flow object type: {inputcast.ObjectType.ToString()}");

                                    string outputType = null;

                                    switch (inputcast.ObjectType)
                                    {
                                        case DTSObjectType.OT_OUTPUT:
                                            outputType = "Output:";
                                            break;

                                        case DTSObjectType.OT_INPUT:
                                            outputType = "Input";
                                            break;

                                        default:
                                            throw new Exception($"Unrecogized IDTSInput100 type:");
                                    }
                                    Indent();
                                    Print(outputType);
                                    UnIndent();
                                    break;
                            }

                        }
                    }

                    UnIndent();

                    break;

                //--------------------------------------------------------------------------------------------------------------------------------------------------------
                case Microsoft.SqlServer.Dts.Runtime.TaskHost taskHost:
                    Print($"TaskHost: {taskHost.Name}");

                    Indent();   
                    switch (taskHost.InnerObject)
                    {
                        case Microsoft.SqlServer.Dts.Pipeline.Wrapper.MainPipe mainPipe:
                            Print("Microsoft.SqlServer.Dts.Pipeline.Wrapper.MainPipe mainPipe");
                            break;
                    }

                    // Drill down to reuse the work for each Executable type (don't reinvent the wheel.)

                    InterpretPackageComponent(packageComponent: taskHost.InnerObject, layer: layer, currentLevel: currentLevel + 1, taskHostName: taskHost.Name);

                    UnIndent();
                    break;

                //--------------------------------------------------------------------------------------------------------------------------------------------------------
                case Microsoft.SqlServer.Dts.Runtime.Sequence sequence:
                    Indent();
                    Print($"Sequence: {sequence.Name}");

                    // Drill down to reuse the work for each Executable type (don't reinvent the wheel.)

                    InterpretPackageComponent(sequence.Executables, layer, currentLevel + 1);
                    UnIndent();
                    break;

                //--------------------------------------------------------------------------------------------------------------------------------------------------------
                case Microsoft.SqlServer.Dts.Runtime.Executables anotherLayerOfExecutables:
                    Indent();

                    if (anotherLayerOfExecutables == null)
                    {
                        Print("Found a null Executable set.");
                    }
                    else if (anotherLayerOfExecutables.Count == 0)
                    {
                        Print("Found an empty Executable set.");
                    }
                    else
                    {

                        Print($"More Executables ({anotherLayerOfExecutables.Count})");
                        Indent();

                        foreach (var anotherExecutable in anotherLayerOfExecutables)
                        {
                            // Drill down to reuse the work for each Executable type (don't reinvent the wheel.)

                            InterpretPackageComponent(anotherExecutable, layer, currentLevel + 1);
                        }
                        UnIndent();
                    }

                    UnIndent();
                    break;

                //--------------------------------------------------------------------------------------------------------------------------------------------------------
                case Microsoft.SqlServer.Dts.Runtime.ForEachLoop forEachLoop:
                    Indent();
                    Print($"ForEach Loop: {forEachLoop.Name}");

                    // Drill down to reuse the work for each Executable type (don't reinvent the wheel.)

                    InterpretPackageComponent(forEachLoop.Executables, layer + "::Loop", currentLevel + 1);
                    UnIndent();

                    break;

                //--------------------------------------------------------------------------------------------------------------------------------------------------------
                case Microsoft.SqlServer.Dts.Runtime.ForLoop forLoop:
                    Indent();
                    Print($"ForLoop: {forLoop.Name}");

                    // Drill down to reuse the work for each Executable type (don't reinvent the wheel.)

                    InterpretPackageComponent(forLoop.Executables, layer + "::Loop", currentLevel + 1);
                    UnIndent();

                    break;

                //--------------------------------------------------------------------------------------------------------------------------------------------------------
                case Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.ExecuteSQLTask executeSQLTask:
                    string executeSQLTasksStatement = executeSQLTask.SqlStatementSource;
                    string whatconnectionname = executeSQLTask.Connection; 

                    Print($"Task Connection: {whatconnectionname}");
                    Print($"Execute SQL: {executeSQLTasksStatement}");

                    // Try to link sql task to it's connection.  For various reasons this link can be broken

                    ConnectionManager executeSQLTaskConnection = package.Connections[whatconnectionname];

                    if (executeSQLTaskConnection == null)
                    {
                        Print($"Ooops! {whatconnectionname} not found in package connections");
                    }
                    else
                    {
                        Print($"connmgr type: {executeSQLTaskConnection.GetType()}");
                        Print($"Connection: {executeSQLTaskConnection.Name}");

                        switch (executeSQLTaskConnection.InnerObject)
                        {
                            case Microsoft.SqlServer.Dts.Runtime.Wrapper.ConnectionManagerHttp connectionManager:
                                break;

                            case Microsoft.SqlServer.Dts.Runtime.Wrapper.ConnectionManagerAdo connectionManagerAdo:
                                break;

                            case Microsoft.SqlServer.Dts.Runtime.Wrapper.ConnectionManagerAdoNet connectionManagerAdoNet:

                                break;
                            default:
                                break;
                        }
                    }

                    break;

                //--------------------------------------------------------------------------------------------------------------------------------------------------------
                case Microsoft.SqlServer.Dts.Tasks.ScriptTask.ScriptTask scriptTask:
                    Print($"Script: {scriptTask.ScriptProjectName}");

                    foreach (var scriptFile in scriptTask.ScriptStorage.ScriptFiles)
                    {
                        Indent();

                        switch (scriptFile)
                        {
                            case Microsoft.SqlServer.VSTAHosting.VSTAScriptProjectStorage.VSTAScriptFile vstaScriptFile:
                                string scriptBody = vstaScriptFile.Data;
                                Print(scriptBody);
                                break;

                            case System.Collections.DictionaryEntry dictionaryEntry:
                                string key = dictionaryEntry.Key.ToString();
                                Print(key);

                                if (dictionaryEntry.Value is Microsoft.SqlServer.VSTAHosting.VSTAScriptProjectStorage.VSTAScriptFile)
                                {
                                    Microsoft.SqlServer.VSTAHosting.VSTAScriptProjectStorage.VSTAScriptFile embeddedDeeperScript = dictionaryEntry.Value as Microsoft.SqlServer.VSTAHosting.VSTAScriptProjectStorage.VSTAScriptFile;
                                    if (embeddedDeeperScript == null) continue;

                                    string actualEmbeddedFileCode = embeddedDeeperScript.Data;

                                    string[,] ignoreTheseHashesArray = 
                                    {
                                        {"39E58801D61A7333A2F776C1EEA01C62", "Standard Message fire to event log." }
                                    };

                                    // Convert array pairs to keypairs

                                    var ignoreTheseHashesDictionary = ignoreTheseHashesArray.Cast<string>()
                                        .Select((s, i) => new { s, i })
                                        .GroupBy(s => s.i / ignoreTheseHashesArray.GetLength(1))
                                        .ToDictionary(
                                        g => g.First().s,
                                        g => g.Skip(1).Select(i => i.s).ToArray()
                                        );

                                    byte[] scriptMD5Hash = new MD5CryptoServiceProvider().ComputeHash(actualEmbeddedFileCode.ToStream());
                                    string scriptMD5HashAsString = scriptMD5Hash.BytesToHexString();

                                    string[] newmessage;
                                    Indent();

                                    if (ignoreTheseHashesDictionary.TryGetValue(scriptMD5HashAsString, out newmessage))
                                    {
                                        Print(newmessage[1]);
                                    }
                                    else
                                    {
                                        string singlelineFileCode = actualEmbeddedFileCode.Replace("\r\n", "<newline>");
                                        Print($"script (as single line): {singlelineFileCode}");
                                        // Popup: Add to ignore??
                                    }

                                    UnIndent();
                                }

                                break;
                            default:
                                throw new NotImplementedException($"Unrecognized script type {scriptFile.GetType().FullName}");
                        }
                        UnIndent();
                    }
                    break;
                //--------------------------------------------------------------------------------------------------------------------------------------------------------
                case Microsoft.SqlServer.Dts.Tasks.ExecuteProcess.ExecuteProcess executeProcess:
                    Print($"Execute Process: {executeProcess.Executable.ToString()}");
                    break;
                //--------------------------------------------------------------------------------------------------------------------------------------------------------
                case Microsoft.SqlServer.Dts.Tasks.ExpressionTask.ExpressionTask expressionTask:
                    Print($"Expression Task: {expressionTask.Expression}");
                    if (expressionTask.Expression.In("@Message = @[User::IsProd] == 1 ? \"This is running in PRODUCTION\" : \"This is running in NON-PRODUCTION\""))
                        // TODO: parse out expression and traverse items.
                        break;
                    break;
                //--------------------------------------------------------------------------------------------------------------------------------------------------------
                case Microsoft.SqlServer.Dts.Tasks.FileSystemTask.FileSystemTask fileSystemTask:
                    Print($"File System Task Operation: {fileSystemTask.Operation}");
                    Print($"File System Operation Name: {fileSystemTask.OperationName}");
                    Print($"File Op Source: {fileSystemTask.Source}");
                    Print($"File Op Destination/Sink: {fileSystemTask.Destination}");

                    break;
                //--------------------------------------------------------------------------------------------------------------------------------------------------------
                case Microsoft.SqlServer.Dts.Tasks.ExecutePackageTask.ExecutePackageTask executePackageTask:
                    Print($"Execute Package Task: {executePackageTask.PackageName}");
                    break;
                //--------------------------------------------------------------------------------------------------------------------------------------------------------
                case Microsoft.SqlServer.Dts.Tasks.SendMailTask.SendMailTask sendMailTask:
                    Print($"Send Mail Task Subject: {sendMailTask.Subject}");
                    Print($"Send Mail Task To: {sendMailTask.ToLine}");
                    Print($"Send Mail Task CC: {sendMailTask.CCLine}");
                    Print($"Send Mail Task BCC: {sendMailTask.BCCLine}");
                    Print($"Send Mail Task BCC: {sendMailTask.MessageSource}");
                    break;
                //--------------------------------------------------------------------------------------------------------------------------------------------------------
                case Microsoft.SqlServer.Dts.Tasks.BulkInsertTask.BulkInsertTask bulkInsertTask:
                    Print($"Bulk Insert Task Source: {bulkInsertTask.SourceConnection}");
                    Print($"Bulk Insert Task Destination: {bulkInsertTask.DestinationConnection}");
                    Print($"Bulk Insert Task SQL: {bulkInsertTask.SqlStatement.ToSingleLine()}");
                    break;
                //--------------------------------------------------------------------------------------------------------------------------------------------------------
                case Microsoft.SqlServer.Dts.Tasks.DataProfilingTask.DataProfilingTask dataProfilingTask:
                    Print($"Data Profiling Task Destination: {dataProfilingTask.Destination}");
                    Print($"Data Profiling Task Destination Type: {dataProfilingTask.DestinationType}");
                    Print($"Data Profiling Task Input XML: {dataProfilingTask.ProfileInputXml}");
                    //Print($"Data Profiling Task Profile Requests: {dataProfilingTask.ProfileRequests.Join(";")}");
                    break;
                //--------------------------------------------------------------------------------------------------------------------------------------------------------
                case Microsoft.SqlServer.Dts.Tasks.FtpTask.FtpTask ftpTask:
                    Print($"FTP Task Connection: {ftpTask.Connection}");
                    Print($"FTP Task Local Path: {ftpTask.LocalPath}");
                    Print($"FTP Task Remote Path: {ftpTask.RemotePath}");
                    Print($"FTP Task Operation: {ftpTask.Operation}");
                    Print($"FTP Task Stop on Failure?: {ftpTask.StopOnOperationFailure}");
                    Print($"FTP Task Operation Name: {ftpTask.OperationName}");
                    Print($"FTP Task Connection - ASCII?: {ftpTask.IsTransferTypeASCII}");
                    break;
                //--------------------------------------------------------------------------------------------------------------------------------------------------------
                case Microsoft.SqlServer.Dts.Tasks.MessageQueueTask.MessageQueueTask messageQueueTask:
                    Print($"Message Queue Task Current Package: {messageQueueTask.CurrentExecutingPackageID}");
                    Print($"Message Queue Task DTS Message Package: {messageQueueTask.DTSMessagePackageID}");
                    Print($"Message Queue Task DTS Message Lineage: {messageQueueTask.DTSMessageLineageID}");
                    Print($"Message Queue Task Connection: {messageQueueTask.MsmqConnection}");
                    Print($"Message Queue Task Message Type: {messageQueueTask.MessageType}");
                    Print($"Message Queue Task Message Variables: {messageQueueTask.MessageVariables}");
                    Print($"Message Queue Task Message Data File: {messageQueueTask.MessageDataFile}");
                    break;
                //--------------------------------------------------------------------------------------------------------------------------------------------------------
                case Microsoft.SqlServer.Dts.Tasks.TransferDatabaseTask.TransferDatabaseTask transferMailTask:
                    Print($"Transfer Mail Task:");
                    break;
                //--------------------------------------------------------------------------------------------------------------------------------------------------------
                case Microsoft.SqlServer.Dts.Tasks.TransferErrorMessagesTask.TransferErrorMessagesTask transferErrorMessagesTask:
                    Print($"Transfer Error Messages Task:");
                    break;
                //--------------------------------------------------------------------------------------------------------------------------------------------------------
                case Microsoft.SqlServer.Dts.Tasks.TransferJobsTask.TransferJobsTask transferJobsTask:
                    Print($"Transfer Jobs Task:");
                    break;
                //--------------------------------------------------------------------------------------------------------------------------------------------------------
                case Microsoft.SqlServer.Dts.Tasks.TransferLoginsTask.TransferLoginsTask transferLoginsTask:
                    Print($"Transfer Logins Task:");
                    break;
                //--------------------------------------------------------------------------------------------------------------------------------------------------------
                case Microsoft.SqlServer.Dts.Tasks.TransferObjectsTask.TransferObjectsTask transferObjectsTask:
                    Print($"Transfer Objects Task:");
                    break;
                //--------------------------------------------------------------------------------------------------------------------------------------------------------
                case Microsoft.SqlServer.Dts.Tasks.TransferSqlServerObjectsTask.TransferSqlServerObjectsTask transferSqlServerObjectsTask:
                    Print($"Transfer SQL Server Objects Task:");
                    break;
                //--------------------------------------------------------------------------------------------------------------------------------------------------------
                case Microsoft.SqlServer.Dts.Tasks.TransferStoredProceduresTask.TransferStoredProceduresTask transferStoredProceduresTask:
                    Print($"Transfer Stored Procedures Task:");
                    break;
                //--------------------------------------------------------------------------------------------------------------------------------------------------------
                case Microsoft.SqlServer.Dts.Tasks.WebServiceTask.WebServiceTask webServiceTask:
                    Print($"Web Service Task Connection: {webServiceTask.Connection}");
                    Print($"Web Service Task Output Location: {webServiceTask.OutputLocation}");
                    Print($"Web Service Task Output Type: {webServiceTask.OutputType}");
                    Print($"Web Service Task Overwrite WSDL File?: {webServiceTask.OverwriteWsdlFile}");
                    Microsoft.SqlServer.Dts.Tasks.WebServiceTask.DTSWebMethodInfo webMethodInfo = webServiceTask.WebMethodInfo;
                    if (webMethodInfo != null)
                    {
                        Print($"Web Service Task Web Method Info Message Name: {webServiceTask.WebMethodInfo.MessageName}");
                        Print($"Web Service Task Web Method Info Documentation: {webServiceTask.WebMethodInfo.Documentation}");
                        Print($"Web Service Task Web Method Info Method Name: {webServiceTask.WebMethodInfo.MethodName}");
                    }
                    break;
                //--------------------------------------------------------------------------------------------------------------------------------------------------------
                case Microsoft.SqlServer.Dts.Tasks.WmiDataReaderTask.WmiDataReaderTask wmiDataReaderTask:
                    Print($"$WMI Data Reader Task Output Type: {wmiDataReaderTask.OutputType}");

                    break;
                //--------------------------------------------------------------------------------------------------------------------------------------------------------
                case Microsoft.SqlServer.Dts.Tasks.WmiEventWatcherTask.WmiEventWatcherTask wmiEventWatcherTask:
                    Print($"WMI Event Watcher Task WMI Connection: {wmiEventWatcherTask.WmiConnection}");
                    Print($"WMI Event Watcher Task Wql Query Source: {wmiEventWatcherTask.WqlQuerySource}");
                    Print($"WMI Event Watcher Task Action At Event: {wmiEventWatcherTask.ActionAtEvent}");

                    break;
                //--------------------------------------------------------------------------------------------------------------------------------------------------------
                case Microsoft.SqlServer.Dts.Tasks.XMLTask.XMLTask xmlTask:
                    Print($"XML Task Operation Type: {xmlTask.OperationType}");
                    Print($"XML Task Destination Type: {xmlTask.DestinationType}");
                    Print($"XML Task Diff Algorithm: {xmlTask.DiffAlgorithm}");
                    Print($"XML Task Diff Gram Destination Type: {xmlTask.DiffGramDestinationType}");
                    break;

                //case Microsoft.SqlServer.IntegrationService.HadoopTasks.Tasks.HadoopFileSystemTask
                //case Microsoft.SqlServer.Management.DatabaseMaintenance.DbMaintenanceTSQLExecuteTask
                //case Microsoft.SqlServer.Management.DatabaseMaintenance.DbMaintenanceNotifyOperatorTask
                //case Microsoft.SqlServer.Management.DatabaseMaintenance.DbMaintenanceUpdateStatisticsTask
                //case Microsoft.SqlServer.Management.DatabaseMaintenance.DbMaintenanceBackupTask
                //case Microsoft.SqlServer.Management.IntegrationServices.Catalog.GetObjectFactory()
                //case Microsoft.SqlServer.Management.IntegrationServices.CustomLoggingLevel

                //case Microsoft.SqlServer.Management.IntegrationServices.IntegrationServices

                //case Microsoft.SqlServer.Dts.ASTask.ASTask asTask:
                // Analysis Services Execute DDL Task
                // Data Mining Query Task
                //case Microsoft.SqlServer.Dts.Tasks.LookupTask
                //case Microsoft.SqlServer.Management.Smo.Agent.Job

                default:
                    var packcomtype = packageComponent.ToString();

                    if (packageComponent.ToString() == "System.__ComObject")
                    {
                        Print($"Custom object: Missing task {taskHostName}");
                    }
                    else
                    {
                        Print($"Need to implement {packcomtype}");
                    }
                    break;
            }


        }
    }
    class CleanUpScript
    {
        public CleanUpScript(string find, string printinstead)
        {
            this.find = find;
            this.printinstead = printinstead;


        }
        public string find;
        public string printinstead;
    }
}

/*
 *     RuntimeWrapper.IDTSConnectionManagerDatabaseParameters100 cmParams_Src = CM.InnerObject as RuntimeWrapper.IDTSConnectionManagerDatabaseParameters100;
    OleDbConnection oledbConn_Src = cmParams_Src.GetConnectionForSchema() as OleDbConnection;
    OleDbConnectionStringBuilder oledbCSBuilder_Src = new OleDbConnectionStringBuilder(oledbConn_Src.ConnectionString);
*/