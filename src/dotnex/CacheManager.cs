using System;
using System.IO;
using System.Reflection;
using System.Security;

namespace dotnex
{
    public static class CacheManager
    {
        private const string TEMP_FOLDER_NAME = "dotnex";
        
        private static string _tempFolder = Assembly.GetExecutingAssembly().Location;

        public static int RemoveAllCachedFiles()
        {
            var tempFolder = GetTempFolder();
            try
            {
                if (Directory.Exists(tempFolder))
                {
                    Directory.Delete(tempFolder, true);
                }
                return 0;
            }
            catch (Exception)
            {
                Console.WriteLine("[X] FATAL: Could not remove cached files");
                return 1;
            }
        }

        public static string GetTempFolder()
        {
            try
            {
                _tempFolder = Path.GetTempPath();
            }
            catch (SecurityException)
            {
                Console.WriteLine("[X] Could not obtain access to temp path...");
                Console.WriteLine("[i] Using current directory");
            }
            finally
            {
                _tempFolder = Path.Combine(_tempFolder, TEMP_FOLDER_NAME);
            }
            return _tempFolder;
        }
    }
}