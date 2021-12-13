/*
 * Warning: Built in 4.6.2 Framework after 4.7.2 failed to build.  Not using 4.8 since standard still on 4.7.2. Nugets end up creating duplicates against 4.8.
 * Framework is mandatory since we are using SMO, SSIS, etc.
 * 
 */
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

/*
 * References: (DO NOT EMBED!  default is to embed interop)
 * Build for x86!
 * - C:\Program Files (x86)\Microsoft SQL Server\150\SDK\Assemblies\Microsoft.SqlServer.DTSPipelineWrap.dll - 15.0
 * - C:\Program Files (x86)\Microsoft SQL Server\150\SDK\Assemblies\Microsoft.SQLServer.DTSRuntimeWrap.dll - 15.0
 * - C:\Windows\Microsoft.NET\assembly\GAC_32\Microsoft.SqlServer.SQLTask\v4.0_15.0.0.0__89845dcd8080cc91\Microsoft.SqlServer.SQLTask.dll - 15.0 x86
 * - C:\Program Files (x86)\Microsoft SQL Server\150\SDK\Assemblies\Microsoft.SQLServer.ManagedDTS.dll - 15.0  MUST BE A REFERENCE, but cannot be a USING!!
 * 
 * On github under jeffshumphreys@outlook.com account.
 */
namespace RipSSIS
{
    static class Program
    {
        const int SWP_NOZORDER = 0x4;
        const int SWP_NOACTIVATE = 0x10;

        [DllImport("kernel32")]
        static extern IntPtr GetConsoleWindow();


        [DllImport("user32")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, int flags);

        /// <summary>
        /// Get console output to the next window.  Really helps!  Otherwise it pops up behind.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        static int Main(string[] args)
        {
            Console.WindowWidth     = 50;
            Console.WindowHeight    = 3;
            Console.BufferWidth     = 50;
            Console.BufferHeight    = 3;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;

            var screen = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            var width = screen.Width;
            var height = screen.Height;

            SetWindowPosition(-1500,  200, 1200, 700);
            Console.Title = "Console output for " + Application.ProductName;

            return new MainArgumentDirector().DirectToProcessAccordingToArguments(args);
        }

        public static void SetWindowPosition(int x, int y, int width, int height)
        {
            SetWindowPos(Handle, IntPtr.Zero, x, y, width, height, SWP_NOZORDER | SWP_NOACTIVATE);
        }

        public static IntPtr Handle
        {
            get
            {
                //Initialize(); 
                return GetConsoleWindow();
            }
        }
    }


}
/*
*     DirectoryInfo di = new DirectoryInfo(session["INSTALLLOCATION"] + @"Sources");
FileInfo[] rgFiles = di.GetFiles("*.dtsx");
Microsoft.SqlServer.Dts.Runtime.Application app = new Microsoft.SqlServer.Dts.Runtime.Application();
foreach (FileInfo fi in rgFiles)
{
string pkg = session["INSTALLLOCATION"] + @"Sources\" + fi.Name + "";
Package p = app.LoadPackage(pkg, null); //THIS TAKE 40-60 SECONDE TO EXECUTE       

if (!app.FolderExistsOnDtsServer(@"MSDB\SPU_CUBE_SSIS", session["SSISSERVER"]))
{
app.CreateFolderOnDtsServer("MSDB", "SPU_CUBE_SSIS", session["SSISSERVER"]);
}
if (!app.ExistsOnDtsServer(@"MSDB\SPU_CUBE_SSIS\" + fi.Name + "", session["SSISSERVER"]))
{
app.SaveToDtsServer(p, null, @"MSDB\SPU_CUBE_SSIS\" + fi.Name + "", session["SSISSERVER"]);
}
}
*/