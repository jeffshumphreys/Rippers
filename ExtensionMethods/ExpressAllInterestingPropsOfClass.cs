using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Linq;

using DotNet.Collections.Generic;

using static ExtensionMethods.UtilityFunctions;
using static ExtensionMethods.StringExtensions;

namespace ExtensionMethods
{
    public class ExpressAllInterestingPropsOfClass
    {
        private ExpressAllInterestingPropsOfClass() { }
        private object ObToDecompose { get; }
        public PropertyInfo[] PropertyInfos { get => propertyInfos; set => propertyInfos = value; }
        public PrintDelegate PrintDelegate { get; }
        public string Prefix { get; }

        public ExpressAllInterestingPropsOfClass(object obToDecompose, string prefix = null, PrintDelegate printDelegate = null)
        {
            if (ignorablePropertyValues_Boolean == null)
            { 
                LoadProps(); // LazyLoad?
            }; 

            ObToDecompose = obToDecompose;
            PrintDelegate = printDelegate;

            if (printDelegate == null) printDelegate = Print;  // A default.
            Prefix = prefix;
            if (prefix == null) prefix = obToDecompose.ToString() ?? "{nameundefined}";
        }

        PropertyInfo[] propertyInfos; // Search up the hierarchy
        public ExpressAllInterestingPropsOfClass Decompose()
        {
            propertyInfos = ObToDecompose.GetType().GetProperties(
                              BindingFlags.Public
                            | BindingFlags.Static | BindingFlags.Instance  // Get instance + static
                            | BindingFlags.FlattenHierarchy);

            return this;
        }

        public void ListOut()
        {
            if (propertyInfos == null) Decompose();

            string outline = string.Empty;
            string propertyValue = string.Empty;
            string propertyName = string.Empty;

            foreach (var pi in propertyInfos)
            {
                outline = pi.Name + ": ";
                bool? printit = null;

                switch (pi.GetValue(ObToDecompose))
                {
                    case string propStringValue:
                        if (propStringValue.IsEmpty()) printit = false;  // No reason to print a line of empty, unless say a SqlTask has empty body.
                        else
                        {
                            propertyValue = propStringValue;
                            printit = true;
                        }
                        break;
                    case bool propBoolValue:

                        break;
                }


                outline += propertyValue;
                if (printit == true)
                {
                    PrintDelegate(outline);
                }
            }
        }
        
        private static MultiMapList<string, bool> ignorablePropertyValues_Boolean;
        
        private void LoadProps()
        {
            // https://github.com/Wallsmedia/DotNet.MultiMap
            ignorablePropertyValues_Boolean = new MultiMapList<string, bool>(1000);
            ignorablePropertyValues_Boolean.AddRangeUnique(IGNORABLEPROPERTYVALUES_BOOLEAN);
        }

        // TODO: Move to file

        private object[,] IGNORABLEPROPERTYVALUES_BOOLEAN = new object[, ] {
          { "Disable" ,                                    false }
        , { "Disable" ,                                    false }
        , { "HasExpressions" ,                             false }
        , { "DelayValidation" ,                            false }
        , { "OfflineMode" ,                                false }
        , { "SuspendRequired" ,                            false }
        , { "DelayValidation" ,                            false }
        , { "DelayValidation" ,                            false }
        , {"ApartmentThreaded",                            false } 
        , {"DebugMode",                                    false } 
        , {"DisableEventHandlers",                         false } 
        , {"FailParentOnFailure",                          false } 
        , {"ForceExecutionValue",                          false } 
        , {"SuspendRequired",                              false } 
        , { "IsStoredProcedure",                           false }
        , { "DebugMode" ,                                  false }
        , { "ForceExecutionValue",                         false }
        , {"package.SafeRecursiveProjectPackageExecution", false } 
        , {"DisableEventHandlers",                         false } 
        , {"IgnoreConfigurationsOnLoad",                   false } 
        , {"DumpOnAnyError",                               false } 
        , {"EncryptCheckpoints",                           false } 
        , {"InteractiveMode",                              false } 
        , {"SaveCheckpoints",                              false } 
        , {"SuppressConfigurationWarnings",                false } 
        , {"UpdateObjects",                                false } 
        , {"FailParentOnFailure",                          false } 
        , {"SafeRecursiveProjectPackageExecution",         false } 
        , {"IsSorted",                                     false } 
        , {"Dangling",                                     false } 
        , {"HasSideEffects",                               true  } 
        , {"AreInputColumnsAssociatedWithOutputColumns",   true  } 
        , {"IsAttached",                                   true  } 
        , {"AreInputColumnsValid",                         true  } 
        , {"UsesDispositions",                             true  } 
        , {"ValidateExternalMetadata",                     true  } 
        //, {"EnableConfigurations",                         true  } 
        , {"EnableDump",                                   true  } 
        , {"CheckSignatureOnLoad",                         true  } 
        , {"IsDefaultLocaleID",                            true  } 
        , { "SupportsDTCTransactions",                     true  }
        , { "IsDefaultLocaleID",                           true  }
        , { "EvaluatesTrue",                               true  }
        , { "ConnectUsingManagedIdentity",                 true  }
        , { "BypassPrepare",                               true  }
        , { "LogicalAnd",                                  true  }
        };

        
    }

    public static class ExtendMultiMapList
    {
        public static void AddRangeUnique(this MultiMapList<string, bool> multiMapList, object[,] arrayOfKeyPairValues)
        {
            var GetItIntoForm = new System.Collections.Generic.List<bool>(1);
            foreach (object[] pair in arrayOfKeyPairValues)
            {
                GetItIntoForm[0] = (bool)pair[1]; // Ugly.
                multiMapList.Add(pair[0] as string,GetItIntoForm);
            }
        }
    }
}

