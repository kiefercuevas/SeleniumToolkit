using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SeleniumToolkit.DriverDownloaders
{
    public class DriverDownLoader : IDisposable
    {
        private static class FileFormat
        {
            public const string ZIP = ".zip";
            public const string GZ = ".gz";
        }
        private readonly WebClient Client;

        public const string ChromeDriverName = "chromedriver";
        public const string EdgeDriverName = "edgedriver";
        public const string MozillaDriverName = "geckodriver";
        public const string OperaDriverName = "operadriver";


        public DriverDownLoader()
        {
            Client = new WebClient();
        }
        public async Task<string> GetLastestDriverVersion(Browser browser)
        {
            try
            {
                string result = await Client.DownloadStringTaskAsync(GetUrl(browser));
                switch (browser)
                {
                    case Browser.MOZILLA:
                        result = GetMozillaVersion(result);
                        break;
                    case Browser.OPERA:
                        result = GetOperaVersion(result);
                        break;
                    default:
                        break;
                }

                return Sanatize(result);
            }
            catch (Exception)
            {
                throw;
            }

        }


        public async Task DownLoadDriver(Browser browser, string version, string directoryPath, DriverType system = DriverType.WIN32)
        {

            try
            {
                string fullPath = Path.Combine(directoryPath, GetDriverName(version, browser, system));
                await Client.DownloadFileTaskAsync(GetDriverUrl(browser, system, version), fullPath);

                string extension = Path.GetExtension(fullPath);
                if (extension.ToLower() == FileFormat.ZIP)
                {
                    UnZipFile(fullPath, browser);
                    File.Delete(fullPath);
                }
                else if (extension.ToLower() == FileFormat.GZ)
                {
                    GZipFile(fullPath, browser);
                    File.Delete(fullPath);
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void DeleteAllDriverFiles(string dirPath, string driverName = ChromeDriverName)
        {
            string[] files = Directory.GetFiles(dirPath);
            foreach (string file in files)
            {
                if (file.ToLower().Contains(driverName))
                {
                    File.Delete(file);
                }
            }
        }


        private static void GZipFile(string fullPath, Browser browser)
        {
            FileInfo gzipFileName = new FileInfo(fullPath);
            string fileName = GetPartialDriverName(browser);

            using FileStream fileToDecompressAsStream = gzipFileName.OpenRead();
            string decompressedFileName = Path.Combine(Path.GetDirectoryName(fullPath), fileName);
            using FileStream decompressedStream = File.Create(decompressedFileName);
            using GZipStream decompressionStream = new GZipStream(fileToDecompressAsStream, CompressionMode.Decompress);
            try
            {
                decompressionStream.CopyTo(decompressedStream);
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not decompress the gz file \n {ex.Message}");
            }
        }

        private static void UnZipFile(string fullPath, Browser browser)
        {
            using ZipArchive Zip = ZipFile.OpenRead(fullPath);
            ZipArchiveEntry entry = Zip.Entries.FirstOrDefault(n => n.Name.ToLower().Contains(GetPartialDriverName(browser)));

            if (entry != null)
            {
                string driverPath = Path.Combine(Path.GetDirectoryName(fullPath), entry?.Name);
                entry?.ExtractToFile(driverPath);
            }
            else
            {
                throw new Exception("The driver has not been downloaded");
            }
        }


        private static string GetDriverName(string version, Browser browser, DriverType system)
        {
            return browser switch
            {
                //https://github.com/mozilla/geckodriver/releases/download/v0.28.0/geckodriver-v0.28.0-win64.zip
                Browser.CHROME => $"chromedriver_{GetDriverTypeName(system)}",
                Browser.EDGE => $"edgedriver_{GetDriverTypeName(system)}",
                Browser.MOZILLA => $"geckodriver-{version}-{GetDriverTypeName(system)}",
                Browser.OPERA => $"operadriver_{GetDriverTypeName(system)}",
                _ => throw new Exception("The browser is not valid"),
            };
        }
        private static string GetPartialDriverName(Browser browser)
        {
            return browser switch
            {
                Browser.CHROME => ChromeDriverName,
                Browser.EDGE => EdgeDriverName,
                Browser.MOZILLA => MozillaDriverName,
                Browser.OPERA => OperaDriverName,
                _ => throw new Exception("The browser is not valid"),
            };
        }
        private static string GetDriverTypeName(DriverType driverType)
        {
            return driverType switch
            {
                DriverType.ARM64 => "arm64.zip",
                DriverType.WIN32 => "win32.zip",
                DriverType.WIN64 => "win64.zip",
                DriverType.LINUX64 => "linux64.zip",
                DriverType.MAC64 => "mac64.zip",
                DriverType.LINUX32_TAR_GZ => "linux32.tar.gz",
                DriverType.LINUX32_TAR_GZ_ASC => "linux32.tar.gz.asc",
                DriverType.LINUX64_TAR_GZ => "linux64.tar.gz",
                DriverType.LINUX64_TAR_GZ_ASC => "linux64.tar.gz.asc",
                DriverType.MACOS => "macos.tar.gz",
                _ => throw new Exception("The System is not valid"),
            };
        }


        private static string GetDriverUrl(Browser browser, DriverType system, string version)
        {
            return browser switch
            {
                Browser.CHROME => $"https://chromedriver.storage.googleapis.com/{version}/{GetDriverName(version, browser, system)}",
                Browser.EDGE => $"https://msedgedriver.azureedge.net/{version}/{GetDriverName(version, browser, system)}",
                Browser.MOZILLA => $"https://github.com/mozilla/geckodriver/releases/download/{version}/{GetDriverName(version, browser, system)}",
                Browser.OPERA => $"https://github.com/operasoftware/operachromiumdriver/releases/download/{version}/{GetDriverName(version, browser, system)}",
                _ => throw new Exception("The browser is not valid"),
            };
        }
        private static string GetUrl(Browser browser)
        {
            return browser switch
            {
                Browser.CHROME => "https://chromedriver.storage.googleapis.com/LATEST_RELEASE",
                Browser.EDGE => "https://msedgedriver.azureedge.net/LATEST_STABLE",
                Browser.MOZILLA => "https://github.com/mozilla/geckodriver/releases/latest",
                Browser.OPERA => "https://github.com/operasoftware/operachromiumdriver/releases/latest",
                _ => throw new Exception("The browser is not valid"),
            };
        }

        private static string GetMozillaVersion(string html)
        {
            Regex stringHtml = new Regex(@"https://github.com/mozilla/geckodriver/releases/tag/(v|V){1}.?(\d+.?)+");
            Match match = stringHtml.Match(html);

            return string.IsNullOrWhiteSpace(match.Value) ? string.Empty : Path.GetFileName(match.Value);
        }
        private static string GetOperaVersion(string html)
        {
            //
            Regex stringHtml = new Regex(@"https://github.com/operasoftware/operachromiumdriver/releases/tag/(v|V){1}.?(\d+.?)+");
            Match match = stringHtml.Match(html);

            return string.IsNullOrWhiteSpace(match.Value) ? string.Empty : Path.GetFileName(match.Value);
        }

        private static string Sanatize(string value)
        {
            return value.Replace("\n", string.Empty)
                        .Replace("\r", string.Empty)
                        .Replace("&", string.Empty);
        }

        public void Close()
        {
            Dispose();
        }
        public void Dispose()
        {
            Client.Dispose();
        }
    }

    /// <summary>
    /// An enum that represent the file type to download
    /// </summary>
    public enum DriverType
    {
        /// <summary>File type valid for Edge browser</summary>
        ARM64,
        /// <summary>File type valid for Edge and Chrome browser</summary>
        WIN32,
        /// <summary>File type valid for Edge browser</summary>
        WIN64,
        /// <summary>File type valid for Mozilla browser</summary>
        LINUX32_TAR_GZ,
        /// <summary>File type valid for Mozilla browser</summary>
        LINUX32_TAR_GZ_ASC,
        /// <summary>File type valid for Chrome and Mozilla browser</summary>
        LINUX64,
        /// <summary>File type valid for Mozilla browser</summary>
        LINUX64_TAR_GZ,
        /// <summary>File type valid for Mozilla browser</summary>
        LINUX64_TAR_GZ_ASC,
        /// <summary>File type valid for Chrome, Opera and Mozilla browser</summary>
        MAC64,
        /// <summary>File type valid for Mozilla browser</summary>
        MACOS,
    }
    public enum Browser
    {
        CHROME,
        EDGE,
        MOZILLA,
        OPERA,
    }
    
}
