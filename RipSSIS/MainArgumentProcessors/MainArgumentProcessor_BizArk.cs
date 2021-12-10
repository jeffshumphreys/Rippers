using System;
using System.Threading.Tasks;
using System.Text;
using System.Reflection;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

using BizArk.ConsoleApp;
using BizArk.Core.Extensions.AttributeExt;
using BizArk.Core.Extensions.EnumerableExt;
using BizArk.Core.Extensions.FormatExt;
using BizArk.Core.Extensions.StringExt;
using BizArk.Core.Util;
using BizArk.Core;

namespace RipSSIS
{
    [CmdLineOptions(nameof(PackagePath))]
    public class MainArgumentProcessor_BizArk : BaseConsoleApp
    {
        
        public static MainArgumentProcessor_BizArk fakeArgumentHolder = null;

        // https://github.com/BizArk/BizArk3/wiki
        // https://github.com/BizArk/BizArk3/wiki/Command-line-Parsing-with-BaCon
        public MainArgumentProcessor_BizArk()
        {
            // Setup the default values.
        }

        public MainArgumentProcessor_BizArk(string PackagePath = null, bool? BatchMode = null)
        {
            this.PackagePath = PackagePath;
            this.BatchMode = BatchMode ?? false;
        }

        [StringLength(800, MinimumLength = 2)]
        [Description("Path to the package dtsx file, including the name and the '.dtsx' on the end. Max 800.")]
        public string PackagePath { get; set; }
        public bool BatchMode { get; set;}

        public override int Start()
        {
            if (fakeArgumentHolder != null)
            {
                PackagePath = fakeArgumentHolder.PackagePath;
                BatchMode = fakeArgumentHolder.BatchMode;
            }

            BaCon.WriteLine("PackagePath={0}".Fmt(PackagePath), ConsoleColor.White);

            new ProcessUserRequest(this);
            return 0;
        }
    }
}
