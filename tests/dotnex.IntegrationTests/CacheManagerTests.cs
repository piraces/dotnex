using FluentAssertions;
using System.IO;
using Xunit;

namespace dotnex.IntegrationTests
{
    public class CacheManagerTests
    {
        [Fact]
        public void TempFolderShouldBeUserTemporaryFolder()
        {
            var tempFolder = CacheManager.GetTempFolder();
            var expectedPath = Path.Combine(Path.GetTempPath(), "dotnex");
            tempFolder.Should().Be(expectedPath);
        }

        [Fact]
        public void RemoveAllCachedFilesShouldRemoveAllTempFolder()
        {
            // ASSERT
            var tempFolder = CacheManager.GetTempFolder();
            Directory.CreateDirectory(tempFolder);
            // Create random file inside temp folder
            string fileName = Path.GetRandomFileName();
            var pathStringNewFile = Path.Combine(tempFolder, fileName);
            var fileStreamNewFile = File.Create(pathStringNewFile);
            fileStreamNewFile.Close();
            // Create random subdirectory
            var pathStringSubdirectory = Path.Combine(tempFolder, Path.GetRandomFileName());
            Directory.CreateDirectory(pathStringSubdirectory);

            // ACT
            CacheManager.RemoveAllCachedFiles();

            // ASSERT
            var tempFolderExists = Directory.Exists(tempFolder);
            var tempFileExists = Directory.Exists(pathStringNewFile);
            var tempSubdirectoryExists = Directory.Exists(pathStringSubdirectory);

            tempFolderExists.Should().BeFalse();
            tempFileExists.Should().BeFalse();
            tempSubdirectoryExists.Should().BeFalse();
        }
    }
}