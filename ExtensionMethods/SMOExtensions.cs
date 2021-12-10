using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.SqlServer.Management.Smo;
using System.Collections.Specialized;
using System.Threading;
using Xunit;
using Microsoft.Collections.Extensions;

namespace ExtensionMethods
{
    public static class SMOExtensions
    {
        // ――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――
        // Way to prevent updates to GUI from overrunning each other. So if call A needs to set flag B, and call B needs flag B to be set, but call B starts before call A finishes, we would get an error. So I lock call A to prevent the run ahead.
        // ――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――
        public class Lock
        {
            public Lock (int px)
            {
                x = px;
            }
            int x;
        }

        public static StringComparison defaultSMOStringComparison  = StringComparison.OrdinalIgnoreCase;
        public static Version SS2000 = new Version (8, 0);
        public static Version SS2005 = new Version (9, 0);
        public static Version SS2008 = new Version (10, 0);
        public static Version SS2008R2 = new Version (10, 50);
        public static Version SS2012 = new Version (11, 0);
        public static Version SS2014 = new Version (12, 0);

        public static bool IsIdenticalTo (this Table sourcetable, ColumnCollection targetColumns, ref MultiValueDictionary<String, String> pointsOfDifference, bool ignoreCollationDifference = true, bool ignoreTargetNullability = false, bool ignoreTargetWider = false, bool ignoreDefaultMissing = true
            , string[] filtercolumnlist = null)
        {
            Monitor.Enter (targetColumns);
            Lock x = new Lock (9);
            bool differ = false;

            int sourcecolumnfilteredcount = 0;
            if ( filtercolumnlist != null && filtercolumnlist.Count () > 0 )
            {
                foreach ( Microsoft.SqlServer.Management.Smo.Column c in sourcetable.Columns )
                {
                    if ( c.Name.In (StringComparison.OrdinalIgnoreCase, filtercolumnlist) )
                    {
                        sourcecolumnfilteredcount++;
                    }
                }
            }
            else
            {
                sourcecolumnfilteredcount = sourcetable.Columns.Count;
            }
            if ( sourcecolumnfilteredcount != targetColumns.Count )
            {
                differ = true; pointsOfDifference.Add (sourcetable.Name, "Column count differs");
            }

            int si = 0;
            Database sourcedb = sourcetable.Parent;
            Database targetdb = ((Table)targetColumns.Parent).Parent;

            foreach ( Column sourceColumn in sourcetable.Columns )
            {
                if ( filtercolumnlist != null && !sourceColumn.Name.In (StringComparison.OrdinalIgnoreCase, filtercolumnlist) )
                {
                    continue;
                }
        retryGetDataType:
                try
                {
                    if ( !targetColumns.Contains (sourceColumn.Name) ) { differ = true; pointsOfDifference.Add (sourceColumn.Name, "Source Column " + sourceColumn.Name + " missing from target."); continue; }
                    Column targetColumn = targetColumns [sourceColumn.Name];
                    if ( targetColumns [si] != targetColumn ) { differ = true; pointsOfDifference.Add (sourceColumn.Name, "Source Column " + sourceColumn.Name + " position # " + si + " is mislocated from target (#" + targetColumn.ID + ")"); }

                    string sourcebasetype = sourcedb.UserDefinedDataTypes [sourceColumn.DataType.Name] != null ? sourcedb.UserDefinedDataTypes [sourceColumn.DataType.Name].SystemType : sourceColumn.DataType.Name;
                    string targetbasetype = targetdb.UserDefinedDataTypes [targetColumn.DataType.Name] != null ? targetdb.UserDefinedDataTypes [targetColumn.DataType.Name].SystemType : targetColumn.DataType.Name;

                    if ( ignoreTargetWider ) // Interesting thought: In C++ we can override the != and <= ops. We could have != subsume when the source is <= to the target size. Or create new op meaning "Significant difference".
                    {
                        // Examine UNDERLYING data types
                        if ( sourcebasetype != targetbasetype )
                        {
                            if ( !(
                                ( sourcebasetype.In (defaultSMOStringComparison, "SMALLDATETIME", "DATE", "DATETIME", "DATETIME2", "DATETIME2(7)") && targetbasetype.In (defaultSMOStringComparison, "DATETIME2", "DATETIME2(7)") )
                                ||
                                ( sourcebasetype.In (defaultSMOStringComparison, "SMALLDATETIME", "DATE", "DATETIME") && targetbasetype.In (defaultSMOStringComparison, "DATETIME", "DATETIME2", "DATETIME2(7)") )
                                ||
                                ( sourcebasetype.In (defaultSMOStringComparison, "DATE") && targetbasetype.In (defaultSMOStringComparison, "DATE", "DATETIME", "DATETIME2", "DATETIME2(7)") )
                                ||
                                ( sourcebasetype.In (defaultSMOStringComparison, "NUMERIC", "DECIMAL") && targetbasetype.In (defaultSMOStringComparison, "DECIMAL", "NUMERIC", "INT", "BIGINT", "TINYINT")
                                        && sourceColumn.DataType.NumericPrecision <= targetColumn.DataType.NumericPrecision && sourceColumn.DataType.NumericScale <= targetColumn.DataType.NumericScale )
                                ) )
                            {
                                differ = true;
                                pointsOfDifference.Add (sourceColumn.Name, "Source Column " + sourceColumn.Name + " data type " + sourcebasetype + " doesn't fit in underlying target type (" + targetbasetype + ")");
                            }
                        }
                        else
                        {
                            if ( sourceColumn.DataType.MaximumLength > targetColumn.DataType.MaximumLength ) { differ = true; pointsOfDifference.Add (sourceColumn.Name, "Source Column " + sourceColumn.Name + " data type max len " + sourceColumn.DataType.MaximumLength + " doesn't fit in target type (" + targetColumn.DataType.MaximumLength + ")"); }
                            if ( sourceColumn.DataType.NumericPrecision > targetColumn.DataType.NumericPrecision ) { differ = true; pointsOfDifference.Add (sourceColumn.Name, "Source Column " + sourceColumn.Name + " data type precision " + sourceColumn.DataType.NumericPrecision + " doesn't fit in target type (" + targetColumn.DataType.NumericPrecision + ")"); }
                            if ( sourceColumn.DataType.NumericScale > targetColumn.DataType.NumericScale ) { differ = true; pointsOfDifference.Add (sourceColumn.Name, "Source Column " + sourceColumn.Name + " data type scale " + sourceColumn.DataType.NumericScale + " doesn't fit in target type (" + targetColumn.DataType.NumericScale + ")"); }
                        }
                    }
                    else
                    {
                        if ( sourcebasetype != targetbasetype ) { differ = true; pointsOfDifference.Add (sourceColumn.Name, "Source Column " + sourceColumn.Name + " data type " + sourcebasetype + " doesn't match target type (" + targetbasetype + ")"); }
                        if ( sourceColumn.DataType.MaximumLength != targetColumn.DataType.MaximumLength ) { differ = true; pointsOfDifference.Add (sourceColumn.Name, "Source Column " + sourceColumn.Name + " data type max len " + sourceColumn.DataType.MaximumLength + " doesn't match target type (" + targetColumn.DataType.MaximumLength + ")"); }
                        if ( sourceColumn.DataType.NumericPrecision != targetColumn.DataType.NumericPrecision ) { differ = true; pointsOfDifference.Add (sourceColumn.Name, "Source Column " + sourceColumn.Name + " data type precision " + sourceColumn.DataType.NumericPrecision + " doesn't match target type (" + targetColumn.DataType.NumericPrecision + ")"); }
                        if ( sourceColumn.DataType.NumericScale != targetColumn.DataType.NumericScale ) { differ = true; pointsOfDifference.Add (sourceColumn.Name, "Source Column " + sourceColumn.Name + " data type scale " + sourceColumn.DataType.NumericScale + " doesn't match target type (" + targetColumn.DataType.NumericScale + ")"); }
                    }

                    if ( sourceColumn.Default != targetColumn.Default )
                    {
                        if ( !( ignoreDefaultMissing && targetColumn.Default == null ) )      // We really don't want to try and push bizarre defaults to the ETL server. We won't be creating data!
                        {
                            differ = true; pointsOfDifference.Add (sourceColumn.Name, "Source Column " + sourceColumn.Name + " default " + sourceColumn.DataType.Name + " doesn't match target type (" + targetColumn.DataType + ")");
                        }
                    }
                    if ( sourceColumn.Collation != targetColumn.Collation ) { if ( !ignoreCollationDifference ) differ = true; pointsOfDifference.Add (sourceColumn.Name, "Source Column " + sourceColumn.Name + " collation " + sourceColumn.Collation + " doesn't match target type (" + targetColumn.Collation + ")"); }
                    if ( !ignoreTargetNullability )
                    {
                        if ( sourceColumn.Nullable != targetColumn.Nullable ) { if ( !ignoreTargetNullability ) differ = true; pointsOfDifference.Add (sourceColumn.Name, "Source Column " + sourceColumn.Name + " nullable " + sourceColumn.Nullable.ToString () + " doesn't match target type (" + targetColumn.Nullable.ToString () + ")"); };
                    }
                    if ( 1 == 0 )
                    {
                        if ( sourceColumn.Computed != targetColumn.Computed ) { differ = true; pointsOfDifference.Add (sourceColumn.Name, "Source Column " + sourceColumn.Name + " computed " + sourceColumn.Computed.ToString () + " doesn't match target type (" + targetColumn.Computed.ToString () + ")"); };
                        if ( sourceColumn.ComputedText != targetColumn.ComputedText ) { differ = true; pointsOfDifference.Add (sourceColumn.Name, "Source Column " + sourceColumn.Name + " computed text " + sourceColumn.ComputedText + "doesn't match target type (" + targetColumn.ComputedText + ")"); }
                        //column.ExtendedProperties

                        if ( sourceColumn.DefaultConstraint != null && targetColumn.DefaultConstraint == null ) { differ = true; pointsOfDifference.Add (sourceColumn.Name, "Source Column " + sourceColumn.Name + " default constraint " + sourceColumn.DefaultConstraint.Name + " not present in target"); }
                        if ( sourceColumn.DefaultConstraint == null && targetColumn.DefaultConstraint != null ) { differ = true; pointsOfDifference.Add (sourceColumn.Name, "Target Column " + targetColumn.Name + " default constraint " + targetColumn.DefaultConstraint.Name + " not present in source"); }
                        if ( sourceColumn.DefaultConstraint != null && targetColumn.DefaultConstraint != null )
                        {
                            if ( sourceColumn.DefaultConstraint.Text.ToUpperInvariant () != targetColumn.DefaultConstraint.Text.ToUpperInvariant () && "(" + sourceColumn.DefaultConstraint.Text.ToUpperInvariant () + ")" != targetColumn.DefaultConstraint.Text.ToUpperInvariant () )
                            {
                                differ = true; pointsOfDifference.Add (sourceColumn.Name, "Source Column " + sourceColumn.Name + " default constraint text  " + sourceColumn.DefaultConstraint.Text + " doesn't match target type (" + targetColumn.DefaultConstraint.Text + ")");
                            }
                        }

                        if ( sourceColumn.IsPersisted != targetColumn.IsPersisted ) { differ = true; pointsOfDifference.Add (sourceColumn.Name, "Source Column " + sourceColumn.Name + " persistence " + sourceColumn.IsPersisted.ToString () + " doesn't match target type (" + targetColumn.IsPersisted.ToString () + ")"); }
                        if ( sourceColumn.RowGuidCol != targetColumn.RowGuidCol ) { differ = true; pointsOfDifference.Add (sourceColumn.Name, "Source Column " + sourceColumn.Name + " of type ROWGUIDCOL " + sourceColumn.DataType.Name + " doesn't match target type (" + targetColumn.DataType + ")"); }
                    }
                }
                catch ( Exception ee )
                {
                    if ( ee.Message.Contains ("Get DataType failed for Column") )
                    {
                        goto retryGetDataType;
                    }
                }
                si++;
            }
            Monitor.Exit (targetColumns);
            return !differ;
        }

        public static Table CopyFrom (this Table sourcetable, string newName, bool dropIfDifferent = true, ScriptingOptions so = null
            , string newSchema = null, Database targetdb = null, bool copyPKOver = true
            , bool forceTargetToPageCompression = false, bool forceTargetDatasetNametoUpper = false, string tag = null, string tablespace = null, string[] filtercolumnlist = null
            )
        {
            Monitor.Enter (sourcetable);
            Lock lk = new Lock (33);
            Database db = null;

            if ( targetdb != null )
            {
                db = targetdb;
            }
            else
            {
                db = sourcetable.Parent;
            }

            string schema = sourcetable.Schema;
            if ( newSchema != null && newSchema.IsNotEmptyOrWhiteSpace () )
            {
                schema = newSchema;
            }

            if ( !db.Schemas.Contains (schema) )
            {
                db.Schemas.Add (new Schema (db, schema));
            }

            // Same server, database, schema - Then can't be same name!
            if ( schema == sourcetable.Schema && db.Name == sourcetable.Parent.Name && db.Parent.Name == sourcetable.Parent.Parent.Name )

                Assert.NotEqual(sourcetable.Name, newName);

            db.Tables.Refresh (); // I think it gets out of wack, thinking a table exists even thoughh it was just deleted, and so then the DROP fails.

            if ( db.Tables.Contains (newName, schema) )
            {
                Table existing_new_t = db.Tables [newName, schema];
                var differences = new MultiValueDictionary<String, String> ();
                if ( !sourcetable.IsIdenticalTo (existing_new_t.Columns, ref differences) )
                {
                    foreach ( var difference in differences )
                        Debug.WriteLine (differences);

                    try
                    {
                        existing_new_t.Drop ();
                    }
                    catch ( Exception ee )
                    {
                        Debug.Print (ee.Message);
                        throw new Exception ("Error when trying to drop table " + schema + "." + newName + ":", ee);
                    }
                }
                else
                {
                    return existing_new_t;
                }
            }

            Version sourceinstanceversion = sourcetable.Parent.Parent.Version;
            Table targettable = new Table (db,
                forceTargetDatasetNametoUpper ? newName.ToUpperInvariant () : newName, schema);

            FileGroupCollection targetfilegroups = targettable.Parent.FileGroups;
            CopyColumns (targettable, sourcetable, tag, filtercolumnlist);
            Index i2 = null;

            List<string> targeticols = new List<string>();
            if ( copyPKOver )
            {
                foreach ( Index i in targettable.Indexes )
                {
                    string icols = "";
                    foreach ( IndexedColumn c in i.IndexedColumns )
                    {
                        icols += ( icols != "" ? "+" : "" ) + c.Name;
                    }
                    targeticols.Add (icols);
                }

                // Copy primary key, but NOT alternate keys, since deletions can cause other keys to duplicate and block feeds (as I leave the deleted items in the target marked).

                foreach ( Index i in sourcetable.Indexes )
                {

                    //Index pk = sourcetable.Indexes.Cast<Index>().SingleOrDefault(index => index.IndexKeyType == IndexKeyType.DriPrimaryKey);
                    string newname = targettable.Schema + "::" + targettable.Name;


                    string icols = "";
                    foreach ( IndexedColumn c in i.IndexedColumns )
                    {
                        icols += ( icols != "" ? "+" : "" ) + c.Name;
                    }

                    if ( targeticols.Contains (icols) )
                    {
                        continue;
                    }

                    if ( i.IndexKeyType == IndexKeyType.DriPrimaryKey )
                    {
                        newname += ".pk::";
                    }
                    else if ( i.IndexKeyType == IndexKeyType.DriUniqueKey )
                    {
                        newname += ".ak::";
                    }
                    else
                    {
                        newname += ".ix::";
                    }

                    newname += icols.ReplaceWith (@"(?<!^)([aouie]|y(?![aouiey]))(?!$)", "");
                    newname = newname.TrimToMaxLength (128);

                    if ( targettable.Indexes.Contains (newname) )
                    {
                        continue;
                    }

                    i2 = new Index (targettable, newname);
                    i2.NoAutomaticRecomputation = i.NoAutomaticRecomputation;
                    i2.IsUnique = i.IsUnique;
                    if ( sourceinstanceversion >= SS2008 )
                    {
                        i2.FilterDefinition = i.FilterDefinition;
                    }
                    i2.IgnoreDuplicateKeys = i.IgnoreDuplicateKeys;
                    i2.IndexType = i.IndexType;
                    i2.IndexKeyType = i.IndexKeyType;
                    i2.IsClustered = i.IsClustered;

                    foreach ( IndexedColumn ic in i.IndexedColumns )
                    {
                        IndexedColumn ic2 = new IndexedColumn (i2, ic.Name, ic.Descending);
                        ic2.IsIncluded = ic.IsIncluded;
                        i2.IndexedColumns.Add (ic2);
                    }

                    try
                    {
                        // Only pks to targets! Don't want to block insertion of deleted rows into source. Case scenario: row with pk id 1 deleted, and row with pk id 2 inserted as a replacement, with the same values except for the pk. Aks will block.

                        if ( i2.IndexKeyType == IndexKeyType.DriPrimaryKey )
                            targettable.Indexes.Add (i2);
                    }
                    catch ( Exception ee2 )
                    {
                        if ( !ee2.Message.Contains ("Err") )
                            throw;
                    }
                }
            }

            if ( sourceinstanceversion >= SS2005 )
            {
                targettable.QuotedIdentifierStatus = sourcetable.QuotedIdentifierStatus;
                targettable.AnsiNullsStatus = sourcetable.AnsiNullsStatus;
                if (targetfilegroups.Contains(sourcetable.TextFileGroup)) {
                    targettable.TextFileGroup = sourcetable.TextFileGroup;
                } else
                {
                    targettable.TextFileGroup = targetfilegroups[0].Name;
                }
            }

            if ( sourceinstanceversion >= SS2008 )
            {
                targettable.MaximumDegreeOfParallelism = 8; // sourcetable.MaximumDegreeOfParallelism;
                targettable.LockEscalation = LockEscalationType.Table; // sourcetable.LockEscalation;
                targettable.IsVarDecimalStorageFormatEnabled = true; // sourcetable.IsVarDecimalStorageFormatEnabled;
                if ( tablespace != null )
                {
                    targettable.FileGroup = tablespace;
                }
                else
                {
                    if (targetfilegroups.Contains(sourcetable.FileGroup)) {
                        targettable.FileGroup = sourcetable.FileGroup;
                    } else
                    {
                        targettable.FileGroup = targetfilegroups[0].Name;
                    }
                }

                if (sourcetable.FileStreamFileGroup != "") {
                    targettable.FileStreamFileGroup = sourcetable.FileStreamFileGroup;
                    if (targetfilegroups.Contains(sourcetable.FileStreamFileGroup)) {
                        targettable.FileStreamFileGroup = sourcetable.FileStreamFileGroup;
                    } else
                    {
                        targettable.FileStreamFileGroup = targetfilegroups[0].Name;
                    }
                }

                //targettable.ChangeTrackingEnabled = sourcetable.ChangeTrackingEnabled;
                //targettable.TrackColumnsUpdatedEnabled = sourcetable.TrackColumnsUpdatedEnabled;
                Assert.Equal (1, sourcetable.PhysicalPartitions.Count);
                Assert.Equal (0, targettable.PhysicalPartitions.Count);
                PhysicalPartition part = sourcetable.PhysicalPartitions [0];
                var po = part.Parent;
                int pno = part.PartitionNumber;
                if ( forceTargetToPageCompression )
                    targettable.PhysicalPartitions.Add (new PhysicalPartition (targettable, 1, DataCompressionType.Page)); // ColumnStore??
                else
                    targettable.PhysicalPartitions.Add (new PhysicalPartition (targettable, 1, part.DataCompression));
                targettable.OnlineHeapOperation = sourcetable.OnlineHeapOperation;
            }
            else
            {
                targettable.PhysicalPartitions.Add (new PhysicalPartition (targettable, 1, DataCompressionType.Page));
            }

            if ( sourceinstanceversion >= SS2012 )
            {
                if ( sourcetable.IsFileTable )
                {
                    targettable.IsFileTable = sourcetable.IsFileTable;
                    targettable.FileTableNamespaceEnabled = sourcetable.FileTableNamespaceEnabled;
                    targettable.FileTableDirectoryName = sourcetable.FileTableDirectoryName;
                    targettable.FileTableNameColumnCollation = sourcetable.FileTableNameColumnCollation;
                }
            }

            if ( sourceinstanceversion >= SS2014 )
            {
                //copiedtable.Durability
            }

            try
            {
                targettable.Create (); // Have to create FIRST if we want partitioning to pick up.
                //if (i2 != null && !copyPKOver) i2.Disable ();
            }
            catch ( SmoException smoex )
            {
                Exception ex = smoex.InnerException;
                while ( ex != null )
                {
                    Debug.WriteLine (ex.Message);
                    ex = ex.InnerException;
                    // Occurs when server restarted: A severe error occurred on the current command.  The results, if any, should be discarded.
                    throw ex;
                }
            }

            if ( so != null && so.EnforceScriptingOptions )
            {
                try
                {
                    System.Collections.Specialized.StringCollection sc = targettable.Script (so);
                    //string sql = sc.Cast<string>().ToArray().Join('\n');
                }
                catch ( FailedOperationException fe )
                {
                    throw;
                }
            }

            Monitor.Exit (sourcetable);

            return targettable;
        }

        public static void CopyColumns (this Table ptargetTable, Table psourceTable, string tag, string[] filtercolumnlist)
        {
            Server sourceInstance = psourceTable.Parent.Parent;

            foreach ( Column sourceColumn in psourceTable.Columns )
            {
                // Only copy over columns that were defined in SingleBatchRunFeedSets, if they were defined, otherwise *
                if ( filtercolumnlist != null )  // Don't want to be case-sensitive when we are dealing with semantic differences.
                {
                    if (
                        !sourceColumn.Name.In (StringComparison.OrdinalIgnoreCase, filtercolumnlist)
                        )
                    {
                        continue;
                    }
                }
               
                Column targetColumn = null;
                // http://www.capprime.com/software_development_weblog/2010/03/15/UsingSMOAndFailingToGetTheSqlDataTypeOfAUserDefinedDataType.aspx
                if (sourceColumn.DataType.SqlDataType == DataType.UserDefinedDataType(sourceColumn.DataType.Name).SqlDataType) {
                    Database db = psourceTable.Parent;
                    string basetype = db.UserDefinedDataTypes [sourceColumn.DataType.Name] != null ? db.UserDefinedDataTypes [sourceColumn.DataType.Name].SystemType : sourceColumn.DataType.Name;

                    if ( basetype == "nchar" ) targetColumn = new Column (ptargetTable, sourceColumn.Name, DataType.NChar (sourceColumn.DataType.MaximumLength));
                    else if ( basetype == "char" ) targetColumn = new Column (ptargetTable, sourceColumn.Name, DataType.Char (sourceColumn.DataType.MaximumLength));
                    else if ( basetype == "nvarchar" ) targetColumn = new Column (ptargetTable, sourceColumn.Name, DataType.NVarChar (sourceColumn.DataType.MaximumLength));
                    else if ( basetype == "varchar" ) targetColumn = new Column (ptargetTable, sourceColumn.Name, DataType.VarChar (sourceColumn.DataType.MaximumLength));
                    else if ( basetype == "varbinary" ) targetColumn = new Column (ptargetTable, sourceColumn.Name, DataType.VarBinary (sourceColumn.DataType.MaximumLength));
                    else if ( basetype == "decimal" ) targetColumn = new Column (ptargetTable, sourceColumn.Name, DataType.Decimal (sourceColumn.DataType.NumericPrecision, sourceColumn.DataType.NumericScale));
                    else if ( basetype == "numeric" ) targetColumn = new Column (ptargetTable, sourceColumn.Name, DataType.Numeric (sourceColumn.DataType.NumericPrecision, sourceColumn.DataType.NumericScale));
                    else if ( basetype == "bit" ) targetColumn = new Column (ptargetTable, sourceColumn.Name, DataType.Bit);
                    else if ( basetype == "tinyint" ) targetColumn = new Column (ptargetTable, sourceColumn.Name, DataType.TinyInt);
                    else if ( basetype == "smallint" ) targetColumn = new Column (ptargetTable, sourceColumn.Name, DataType.SmallInt);
                    else if ( basetype == "int" ) targetColumn = new Column (ptargetTable, sourceColumn.Name, DataType.Int);
                    else if ( basetype == "bigint" ) targetColumn = new Column (ptargetTable, sourceColumn.Name, DataType.BigInt);
                    else if ( basetype == "binary" ) targetColumn = new Column (ptargetTable, sourceColumn.Name, DataType.Binary (sourceColumn.DataType.MaximumLength));
                    else if ( basetype == "time" ) targetColumn = new Column (ptargetTable, sourceColumn.Name, DataType.Time (sourceColumn.DataType.NumericScale));
                    else if ( basetype == "timestamp" ) targetColumn = new Column (ptargetTable, sourceColumn.Name, DataType.DateTime);
                    else if ( basetype == "datetime" ) targetColumn = new Column (ptargetTable, sourceColumn.Name, DataType.DateTime);
                    else if ( basetype == "datetime2" ) targetColumn = new Column (ptargetTable, sourceColumn.Name, DataType.DateTime2 (sourceColumn.DataType.NumericScale));
                    else if ( basetype == "date" ) targetColumn = new Column (ptargetTable, sourceColumn.Name, DataType.Date);
                    else if ( basetype == "money" ) targetColumn = new Column (ptargetTable, sourceColumn.Name, DataType.Money);
                    else if ( basetype == "smallmoney" ) targetColumn = new Column (ptargetTable, sourceColumn.Name, DataType.SmallMoney);
                    else if ( basetype == "smalldatetime" ) targetColumn = new Column (ptargetTable, sourceColumn.Name, DataType.DateTime);
                    else if ( basetype == "datetimeoffset" ) targetColumn = new Column (ptargetTable, sourceColumn.Name, DataType.DateTimeOffset (sourceColumn.DataType.NumericScale));
                    else if ( basetype == "float" ) targetColumn = new Column (ptargetTable, sourceColumn.Name, DataType.Float);
                    else if ( basetype == "real" ) targetColumn = new Column (ptargetTable, sourceColumn.Name, DataType.Real);
                    else if ( basetype == "image" ) targetColumn = new Column (ptargetTable, sourceColumn.Name, DataType.VarBinaryMax);
                    else if ( basetype == "varbinarymax" ) targetColumn = new Column (ptargetTable, sourceColumn.Name, DataType.VarBinaryMax);
                    else if ( basetype == "ntext" ) targetColumn = new Column (ptargetTable, sourceColumn.Name, DataType.NVarCharMax);
                    else if ( basetype == "nvarcharmax" ) targetColumn = new Column (ptargetTable, sourceColumn.Name, DataType.NVarCharMax);
                    else if ( basetype == "text" ) targetColumn = new Column (ptargetTable, sourceColumn.Name, DataType.VarCharMax);
                    else if ( basetype == "varcharmax" ) targetColumn = new Column (ptargetTable, sourceColumn.Name, DataType.VarCharMax);
                    else if ( basetype == "variant" ) targetColumn = new Column (ptargetTable, sourceColumn.Name, DataType.Variant);
                    else if ( basetype == "geography" ) targetColumn = new Column (ptargetTable, sourceColumn.Name, DataType.Geography); // untested.
                    else if ( basetype == "geometry" ) targetColumn = new Column (ptargetTable, sourceColumn.Name, DataType.Geometry); // untested.
                    else if ( basetype == "hiearchyid" ) targetColumn = new Column (ptargetTable, sourceColumn.Name, DataType.HierarchyId); // untested.
                    else if ( basetype == "sysname" ) targetColumn = new Column (ptargetTable, sourceColumn.Name, DataType.SysName); // untested.
                    else if ( basetype == "xml" ) targetColumn = new Column (ptargetTable, sourceColumn.Name, DataType.Xml (sourceColumn.DataType.GetType ().Name, "dbo", sourceColumn.DataType.XmlDocumentConstraint)); // untested.
                    else if ( basetype == "uniqueidentifier" ) targetColumn = new Column (ptargetTable, sourceColumn.Name, DataType.UniqueIdentifier);

                        // Not handling User table types yet, or CLR types.

                    else throw new Exception ("Unsupported basetype of userdefineddatatype: " + basetype);
                }
                else if ( sourceColumn.DataType.SqlDataType == DataType.Timestamp.SqlDataType )                                         // Tested Ok 11/20/14
                {
                    targetColumn = new Column (ptargetTable, sourceColumn.Name, DataType.DateTime);                                // Timestamp maps to DateTime not DateTime2
                }
                else
                {
                    targetColumn = new Column (ptargetTable, sourceColumn.Name, sourceColumn.DataType);
                }
                //targetColumn.Collation = sourceColumn.Collation;       // BAD: We don't want the weirdo collations from MeridianSchools, etc., infecting the warehouse. Breaks ETL due to incompatible joins, and 30-second joins take infinite time and crash. Bizzare. Should log and determine if values will be corrupted, or if there are any values that are unique on the source but now will be non-unique on target. (i.e., "Josephine", "JOSEPHINE" would become non-unique on our CI_AS target). But that's as it should be. No uniqueifying based on casing!
                targetColumn.Nullable = sourceColumn.Nullable;
                targetColumn.Computed = sourceColumn.Computed;
                targetColumn.ComputedText = sourceColumn.ComputedText;
                //targetColumn.Default = sourceColumn.Default;
                //column.ExtendedProperties

                /*
                if ( sourceColumn.DefaultConstraint != null )
                {
                    string tabname = ptargetTable.Name;
                    string constrname = sourceColumn.DefaultConstraint.Name + tag;
                    constrname = constrname.TrimToMaxLength (128); // TODO: Note if actually cut anything. New String function that either throws error, or has a ref bool actuallyTrimmedNonSpaces.
                    targetColumn.AddDefaultConstraint (constrname);
                    targetColumn.DefaultConstraint.Text = sourceColumn.DefaultConstraint.Text;
                }
                */
                targetColumn.IsPersisted = sourceColumn.IsPersisted;
                targetColumn.DefaultSchema = sourceColumn.DefaultSchema;
                targetColumn.RowGuidCol = sourceColumn.RowGuidCol;

                if ( sourceInstance.Version >= SS2008R2 )
                {
                    targetColumn.IsFileStream = sourceColumn.IsFileStream;
                    targetColumn.IsSparse = sourceColumn.IsSparse;
                }

                if ( sourceInstance.Version >= SS2008 )
                {
                    targetColumn.IsColumnSet = sourceColumn.IsColumnSet;
                }

                ptargetTable.Columns.Add (targetColumn);
            }
        }
    }


}
