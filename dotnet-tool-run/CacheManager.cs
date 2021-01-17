using System;
using System.IO;

namespace dotnettoolrun
{
    public static class CacheManager
    {
        public static int RemoveAllCachedFiles()
        {
            var tempFolderHandler = new TempFolderHandler();
            try
            {
                if (Directory.Exists(tempFolderHandler.TempFolder))
                {
                    Directory.Delete(tempFolderHandler.TempFolder, true);
                }
                return 0;
            }
            catch (Exception)
            {
                Console.WriteLine("[X] FATAL: Could not remove cached files");
                return 1;
            }
        }
    }
}