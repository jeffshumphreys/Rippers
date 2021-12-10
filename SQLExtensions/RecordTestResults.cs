using System;
using System.Data;
using System.Data.SqlClient;

using static ExtensionMethods.UtilityFunctions;

namespace SQLExtensions
{
    /// <summary>
    /// Write Test results from PackageExpander to a database.
    /// </summary>
    public class RecordTestResults
    {
        public static bool?         logTestResultsToDatabaseConnection;
        public static SqlConnection connection = new SqlConnection();
        public static SqlCommand    AddNewTestResultCommand;

        public RecordTestResults()
        {
            var usr = Environment.UserName;
            string DatabaseForTestResults = "RipSSISTestResults"; 
            try
            {
                connection.ConnectionString = $"Server=.;Database={DatabaseForTestResults};Trusted_Connection=true;";
                connection.Open();
                logTestResultsToDatabaseConnection = true;
                AddNewTestResultCommand = new SqlCommand("AddTestParsePackageResult", connection);
                AddNewTestResultCommand.CommandType = CommandType.StoredProcedure;
                SqlParameter returnnewRowKey = AddNewTestResultCommand.Parameters.Add("ReturnValue", SqlDbType.Int); returnnewRowKey.Direction = ParameterDirection.ReturnValue;
                AddNewTestResultCommand.Parameters.Add("@k", SqlDbType.Int);
                AddNewTestResultCommand.Parameters.Add("@PathToTestPackage", SqlDbType.VarChar, 450);
                AddNewTestResultCommand.Parameters.Add("@ProcessingStage", SqlDbType.VarChar, 50);
                AddNewTestResultCommand.Parameters.Add("@PackageName", SqlDbType.VarChar, 100);
                AddNewTestResultCommand.Parameters.Add("@LoadingMessages", SqlDbType.VarChar, int.MaxValue);
                AddNewTestResultCommand.Parameters.Add("@DebugLibsLoaded", SqlDbType.VarChar, int.MaxValue);
                AddNewTestResultCommand.Parameters.Add("@ErrorMessage", SqlDbType.VarChar, 300);
                AddNewTestResultCommand.Parameters.Add("@VSVersion", SqlDbType.VarChar, 30);
                AddNewTestResultCommand.Parameters.Add("@Able_to_open_from_RipSSISPackage", SqlDbType.Bit);
                AddNewTestResultCommand.Parameters.Add("@PackageSize", SqlDbType.BigInt);
                AddNewTestResultCommand.Parameters.Add("@DateTimeStartedLoadFromXML", SqlDbType.DateTime2, 7);
                AddNewTestResultCommand.Parameters.Add("@DateTimeExitedLoadFromXML", SqlDbType.DateTime2, 7);
                AddNewTestResultCommand.Parameters.Add("@PackageFormatVersion", SqlDbType.Int);

                AddNewTestResultCommand.Prepare();
            }
            catch (SqlException e)
            {
                Print($"Cannot connect to localhost and {DatabaseForTestResults} scenarios. Disabling recording of test runs.");
                logTestResultsToDatabaseConnection = false;
            }
            catch (InvalidOperationException e2)
            {
                Print($"Cannot connect to localhost and {DatabaseForTestResults} scenarios. Disabling recording of test runs.");
                logTestResultsToDatabaseConnection = false;
            }
        }
        public int? RecordTestResultsToDb(
                        int?          RecordToMerge                    = null // null for a new record.
                      , string        PathToTestPackage                = null        // All args are optional, depending on stage
                      , string        PackageName                      = null
                      , string        LoadingMessages                  = null
                      , string        DebugLibsLoaded                  = null
                      , string        ErrorMessage                     = null
                      , string        VSVersion                        = null
                      , bool?         Able_to_open_from_RipSSISPackage = null
                      , long?         PackageSize                      = null
                      , DateTime?     DateTimeStartedLoadFromXML       = null
                      , DateTime?     DateTimeExitededLoadFromXML      = null
                      , int?          PackageFormatVersion             = null
                      , string        ProcessingStage                  = null
            )
        {
            if (logTestResultsToDatabaseConnection == true)
            {
                AddNewTestResultCommand.Parameters["@k"].Value                                = RecordToMerge ?? (object)DBNull.Value;
                AddNewTestResultCommand.Parameters["@PathToTestPackage"].Value                = PathToTestPackage ?? (object)DBNull.Value;
                AddNewTestResultCommand.Parameters["@PackageName"].Value                      = PackageName ?? (object)DBNull.Value;
                AddNewTestResultCommand.Parameters["@LoadingMessages"].Value                  = LoadingMessages ?? (object)DBNull.Value;
                AddNewTestResultCommand.Parameters["@DebugLibsLoaded"].Value                  = DebugLibsLoaded ?? (object)DBNull.Value;
                AddNewTestResultCommand.Parameters["@ErrorMessage"].Value                     = ErrorMessage ?? (object)DBNull.Value;
                AddNewTestResultCommand.Parameters["@VSVersion"].Value                        = VSVersion ?? (object)DBNull.Value;
                AddNewTestResultCommand.Parameters["@Able_to_open_from_RipSSISPackage"].Value = Able_to_open_from_RipSSISPackage ?? (object)DBNull.Value;
                AddNewTestResultCommand.Parameters["@PackageSize"].Value                      = PackageSize ?? (object)DBNull.Value;
                AddNewTestResultCommand.Parameters["@DateTimeStartedLoadFromXML"].Value       = DateTimeStartedLoadFromXML ?? (object)DBNull.Value;
                AddNewTestResultCommand.Parameters["@DateTimeExitedLoadFromXML"].Value        = DateTimeExitededLoadFromXML ?? (object)DBNull.Value;
                AddNewTestResultCommand.Parameters["@PackageFormatVersion"].Value             = PackageFormatVersion ?? (object)DBNull.Value;
                AddNewTestResultCommand.Parameters["@ProcessingStage"].Value                  = ProcessingStage ?? (object)DBNull.Value;

                AddNewTestResultCommand.ExecuteNonQuery();
                int? newkey = Convert.ToInt32(AddNewTestResultCommand.Parameters["ReturnValue"].Value);
                return newkey;
            }
            return null;
        }



    }
}