using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SeleniumToolkit.DriverDownloader.Downloaders
{
    public sealed class ChromeDriverDownloader : DriverDownloader
    {
        private readonly WebClient Client;

        public ChromeDriverDownloader() 
            :base("chromedriver")
        {
            Client = new WebClient();
        }

        public override async Task<string> GetLastestVersion()
        {
            try
            {
                string lastVersion = GetChromeVersion(await Client.DownloadStringTaskAsync("https://chromedriver.chromium.org/downloads"))[0];
                return Sanatize(lastVersion);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override async Task<string[]> GetVersions()
        {
            try
            {
                return GetChromeVersion(await Client.DownloadStringTaskAsync("https://chromedriver.chromium.org/downloads"))
                        .Select(m => Sanatize(m)).ToArray();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override async Task<string> GetStableVersion()
        {
            try
            {
                return Sanatize(await Client.DownloadStringTaskAsync("https://chromedriver.storage.googleapis.com/LATEST_RELEASE"));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override async Task<string> DownLoad(string version, string directoryPath, SystemType system)
        {

            try
            {
                ValidateSystemType(system);

                string fullName = $"{PartialName}_{GetDriverTypeName(system)}";
                string url = $"https://chromedriver.storage.googleapis.com/{version}/{fullName}";
                string fullPath = Path.Combine(directoryPath, fullName);

                await Client.DownloadFileTaskAsync(url, fullPath);

                return fullPath;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override SystemType[] GetSystemTypes()
        {
            return new SystemType[] 
            {
               SystemType.WIN32,
               SystemType.MAC64,
               SystemType.MAC64_M1,
               SystemType.LINUX64
            };
        }

        private string [] GetChromeVersion(string html)
        {
            Regex stringHtml = new Regex(@"ChromeDriver *(\d+\.{1})+\d+");
            var matches = stringHtml.Matches(html)
                .OrderByDescending(m => m.Value)
                .Select(m => m.Value)
                .ToList();

            if (matches.Count == 0)
                throw new Exception("Could not found the driver version");
            return matches.Select(m => m.Replace("ChromeDriver ", "")).ToArray();
        }

        public override void Dispose()
        {
            Client.Dispose();
        }

    }
}
