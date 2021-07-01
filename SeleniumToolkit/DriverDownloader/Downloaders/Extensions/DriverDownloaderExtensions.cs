using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumToolkit.DriverDownloader.Downloaders.Extensions
{
    public static partial class DriverDownloaderExtensions
    {
        /// <summary>
        /// Execute all the process from downloading the driver to UnZip all the files related to it
        /// </summary>
        /// <param name="directoryPath">The directory to download the file</param>
        /// <param name="system"></param>
        /// <returns>Return the file Path of the current driver instance</returns>
        public static async Task<string> DownloadAndUnzip(this IDriverDownloader driverDownloader,
            string directoryPath, SystemType system)
        {
            string filePath = await driverDownloader.DownLoad(await driverDownloader.GetLastestVersion(),
                directoryPath, system);

            driverDownloader.UnZipDriverFile(filePath);
            return filePath;
        }

        /// <summary>
        /// Execute all the process from downloading the driver to UnZip all the files related to it
        /// </summary>
        /// /// <param name="version">The current version to download</param>
        /// <param name="directoryPath">The directory to download the file</param>
        /// <param name="system"></param>
        /// <returns>Return the file Path of the current driver instance</returns>
        public static async Task<string> DownloadAndUnzip(this IDriverDownloader driverDownloader,
            string version, string directoryPath, SystemType system)
        {
            string filePath = await driverDownloader.DownLoad(version, directoryPath, system);
            driverDownloader.UnZipDriverFile(filePath);
            return filePath;
        }

        /// <summary>
        /// Execute all the process from downloading the driver to UnZip all the files related to it
        /// </summary>
        /// <param name="directoryPath">The directory to download the file</param>
        /// <param name="system"></param>
        /// <param name="driverFileCallback">
        /// A callback with the driver file Path as a param</param>
        public static async Task<string> DownloadAndUnzip(this IDriverDownloader driverDownloader,
            string directoryPath, SystemType system, Action<string> driverFileCallback)
        {
            string filePath = await driverDownloader.DownLoad(await driverDownloader.GetLastestVersion(),
                directoryPath, system);

            driverDownloader.UnZipDriverFile(filePath);
            driverFileCallback(filePath);

            return filePath;
        }

        /// <summary>
        /// Unzip all the files inside the gzip filePath
        /// </summary>
        /// <param name="gzipFilePath">The path of the gzip file</param>
        /// <param name="driverName">The name to use to create the driver file</param>
        public static void UnGZipDriverFiles(this IDriverDownloader driverDownloader,  string gzipFilePath, string driverName)
        {
            try
            {
                FileInfo gzipFileName = new FileInfo(gzipFilePath);
                using (FileStream fileToDecompressAsStream = gzipFileName.OpenRead())
                {
                    string decompressedFileName = Path.Combine(Path.GetDirectoryName(gzipFilePath), driverName);
                    using (FileStream fileStream = File.Create(decompressedFileName))
                    {
                        using (GZipStream gzipStream = new GZipStream(fileToDecompressAsStream, CompressionMode.Decompress))
                        {
                            gzipStream.CopyTo(fileStream);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception($"Could not decompress the gz file \n {ex.Message}");
            }
        }

        /// <summary>
        /// Unzip all the files inside the zip filePath that match the Driver Downloader PartialName property
        /// </summary>
        /// <param name="fullPath">The path of the zip file</param>
        public static string UnZipDriverFile(this IDriverDownloader driverDownloader, string fullPath)
        {
            try
            {
                using (ZipArchive Zip = ZipFile.OpenRead(fullPath))
                {
                    ZipArchiveEntry entry = Zip
                        .Entries.FirstOrDefault(n => n.Name.Contains(driverDownloader.PartialName));

                    if (entry != null)
                    {
                        string driverPath = Path.Combine(Path.GetDirectoryName(fullPath), entry?.Name);
                        entry?.ExtractToFile(driverPath);
                        return driverPath;
                    }
                    else
                    {
                        throw new Exception("The driver has not been downloaded");
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// Unzip all the files inside the zip filePath
        /// </summary>
        /// <param name="fullPath">The path of the zip file</param>
        public static void UnZipDriverFiles(this IDriverDownloader driverDownloader, string fullPath)
        {
            try
            {
                using (ZipArchive Zip = ZipFile.OpenRead(fullPath))
                {
                    foreach (ZipArchiveEntry item in Zip.Entries)
                    {
                        string newPath = Path.Combine(Path.GetDirectoryName(fullPath), item?.Name);
                        item?.ExtractToFile(newPath);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// Deletes all driver files that match with the DriverDownloader PatialName property
        /// </summary>
        /// <param name="dirPath">The directory to look for</param>
        public static void DeleteFiles(this IDriverDownloader driverDownloader, string dirPath)
        {
            try
            {
                string[] files = Directory.EnumerateFiles(dirPath)
                    .Where(f => Path.GetFileName(f).StartsWith(driverDownloader.PartialName))
                    .ToArray();

                foreach (string file in files)
                {
                    File.Delete(file);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Deletes all driver Zip or Gzip files that match with the DriverDownloader PatialName property
        /// </summary>
        /// <param name="dirPath">The directory to look for</param>
        public static void DeleteZipFiles(this IDriverDownloader driverDownloader, string dirPath)
        {
            try
            {
                string[] extensions = { ".zip", ".gz" };
                string[] files = Directory.EnumerateFiles(dirPath)
                    .Where(f => Path.GetFileName(f).StartsWith(driverDownloader.PartialName) &&
                                extensions.Contains(Path.GetExtension(f)))
                    .ToArray();

                foreach (string file in files)
                {
                    File.Delete(file);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Looks for the driver file with the Partialname and Rename it
        /// </summary>
        /// <param name="driverDirectory">The directory where the driver file is</param>
        /// <param name="newFileName">the new name for the driver file</param>
        public static void RenameDriverFile(this IDriverDownloader driverDownloader, string driverDirectory, string newFileName)
        {
            string driverFilePath = Directory.EnumerateFiles(driverDirectory)
                .FirstOrDefault(f => Path.GetFileName(f).StartsWith(DriverFileName.GetFileName(driverDownloader.PartialName)));

            if (string.IsNullOrWhiteSpace(driverFilePath))
                throw new Exception($"The driver file with the name {driverDownloader.PartialName} is not in the directory {driverDirectory}");

            string newPath = Path.Combine(driverDirectory, $"{newFileName}{Path.GetExtension(driverFilePath)}");
            try
            {
                File.Move(driverFilePath, newPath);
            }
            catch (Exception)
            {
                throw;
            }

        }

        private static class DriverFileName
        {
            private static readonly IDictionary<string, string> DriverFilesNames =
                new Dictionary<string, string>()
            {
                { DriverNames.Chrome, DriverNames.Chrome },
                { DriverNames.Opera, DriverNames.Opera },
                { DriverNames.Mozilla, DriverNames.Mozilla},
                { "edgedriver", "msedgedriver" },
            };

            public static string GetFileName(string driverName)
            {
                return DriverFilesNames[driverName];
            }
        }
    }
}
