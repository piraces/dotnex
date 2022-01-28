using System;
using System.IO;
using System.Reflection;
using System.Security;

namespace dotnex
{
    /// <summary>
    /// Main manager for the dotnet tools cache.
    /// It caches tools for subsequent or future executions, avoiding downloading it every time
    /// </summary>
    public static class CacheManager
    {
        private const string TEMP_FOLDER_NAME = "dotnex";
        
        private static string _tempFolder = Assembly.GetExecutingAssembly().Location;

        /// <summary>
        /// Removes all cached tools from the cache temporary folder.
        /// </summary>
        /// <returns>0 if cache has been cleared successfully. 1 otherwise</returns>
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

        /// <summary>
        /// Gets the temporary folder to store the cache in.
        /// </summary>
        /// <returns>The current user temp folder or the current directory if the first is not writable/accesible</returns>
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