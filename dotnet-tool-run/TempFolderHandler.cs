using System;
using System.IO;
using System.Reflection;
using System.Security;

namespace dotnettoolrun
{
    public class TempFolderHandler
    {
        private const string TEMP_FOLDER_NAME = "dotnettoolrun";

        public string TempFolder { get; private set; } = Assembly.GetExecutingAssembly().Location;

        public TempFolderHandler()
        {
            InitializeTempFolder();
        }
        
        private void InitializeTempFolder()
        {
            try
            {
                TempFolder = Path.GetTempPath();
            }
            catch (SecurityException)
            {
                Console.WriteLine("[X] Could not obtain access to temp path...");
                Console.WriteLine("[i] Using current directory");
            }
            finally
            {
                TempFolder = Path.Combine(TempFolder, TEMP_FOLDER_NAME);
            }
            
        }
    }
}