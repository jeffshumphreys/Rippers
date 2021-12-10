using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using DocumentFormat.OpenXml.Vml;

namespace ExtensionMethods
{
    // So Other libraries, like ExpressAllInterestingProps can print out without being aware of output.

    public delegate void PrintDelegate(string msg = "\r\n", bool printonlytoscreen = false, bool noindent = false, bool nowrap = false, bool nowraptofile = false);

    public static class UtilityFunctions
    {
        static int           indent            = 0;
        static int           indentincrement   = 2;
        static string        indentspaces      = String.Empty;
        public static bool   printtoscreen     = true;
        public static bool   printtofile       = false;
        public static bool   wraplinestoscreen = false;
        public static string filepathtoprintto;

        public static void PrintToScreen(bool yesno)
        {
            printtoscreen = yesno;
        }

        public static void WrapLinesToScreen(bool yesno)
        {
            wraplinestoscreen = yesno;
        }

        static FileStream outputresultsfilestream;
        static public void PrintToFile(string filepath)
        {
            filepathtoprintto       = filepath;
            outputresultsfilestream = File.Create(filepathtoprintto);
            printtofile             = true;
        }

        static public void SavePrintsToFile()
        {
            if (outputresultsfilestream != null)
            {
                // This is usually called before pausing to show the developer the console screen, so this is needed to force the buffer out to disk.

                outputresultsfilestream.Flush(true);

                // If this is in debug VS mode, and pauses for getkey(), then this needs to be done first, or you can't open the generated file.

                outputresultsfilestream.Close();

                // Not sure if this is necessary to release the filelock.

                outputresultsfilestream.Dispose();
            }
        }

        static public void Print(string msg = "\r\n", bool printonlytoscreen = false, bool noindent = false, bool nowrap = false, bool nowraptofile = false)
        {
            void PrintNex(string toscreen, string tofile)
            {
                if (printtoscreen)
                {
                    if (toscreen == null || toscreen == "\r\n")
                        Console.WriteLine();
                    else
                        Console.WriteLine(toscreen);
                }
                if (!printonlytoscreen)
                    if (printtofile && outputresultsfilestream.CanWrite) (msg + "\r\n").ToStream().CopyTo(outputresultsfilestream);
            }

            // TODO: Major refactor here.

            if (msg == "\r\n")
            {
                PrintNex(null, msg);
                return;
            }

            if (nowrap || !wraplinestoscreen)
            {
                if (!noindent)
                    msg = indentspaces + msg;
                PrintNex(msg, msg);
                return;
            }
            else
            if (wraplinestoscreen)
            {
                string[] lines = msg.Split(80);
                foreach (var line in lines)
                {
                    if (!noindent)
                        msg = indentspaces + line;

                    if (msg.Trim() == string.Empty && line.Length == 80) // Don't print a blank line if the previous line was wrapped.
                        return;
                    msg = msg.ReplaceRegex(@"([^\r])\n", "$1\r\n" + indentspaces);
                    PrintNex(msg, msg);
                }
            }
        }

        static public void Indent()
        {
            indent++;
            indentspaces = new String(' ', indentincrement * indent);
        }

        static public void UnIndent()
        {
            indent--;
            indentspaces = new String(' ', indentincrement * indent);
        }

        /// <summary>
        /// I don't like the Path one for combining "D:\xxx\" and "\nextfolder".  Doesn't work.
        /// </summary>
        /// <param name="basepath"></param>
        /// <param name="extendedpath"></param>
        /// <returns></returns>
        public static string CombineFolders(string basepath, string extendedpath)
        {
            if (extendedpath == String.Empty || extendedpath == @"\") return basepath;

            if (basepath.EndsWith(@"\") && extendedpath.StartsWith(@"\"))
            {
                return basepath + extendedpath.Substring(2);
            }
            if (basepath.EndsWith(@"\") && !extendedpath.StartsWith(@"\"))
            {
                return basepath + extendedpath;
            }
            if (!basepath.EndsWith(@"\") && extendedpath.StartsWith(@"\"))
            {
                return basepath + extendedpath;
            }
            if (!basepath.EndsWith(@"\") && !extendedpath.StartsWith(@"\"))
            {
                return basepath + @"\" + extendedpath.Substring(2);
            }
            throw new Exception("Unable to combine these paths");
        }

        /// <summary>
        /// Not used.  Thought it would tell me if I was running from VS in debug.  Found a way.  This just tells me if it's winforms or console.
        /// </summary>
        /// <param name="_"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll")] static extern IntPtr GetModuleHandleW(IntPtr _);
        public static bool IsGui
        {
            get
            {
                var p = GetModuleHandleW(default);
                return Marshal.ReadInt16(p, Marshal.ReadInt32(p, 0x3C) + 0x5C) == 2;
            }
        }
    }
}
