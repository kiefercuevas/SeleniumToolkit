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
    public class EdgeDriverDownloader :DriverDownloader
    {
        private readonly WebClient Client;
        public EdgeDriverDownloader() 
            :base("edgedriver")
        {
            Client = new WebClient();
        }

        public override async Task<string> GetLastestVersion()
        {
            try
            {
                string latestVersion = GetEdgeVersion(await Client.DownloadStringTaskAsync("https://developer.microsoft.com/en-us/microsoft-edge/tools/webdriver/"))[0];
                return Sanatize(latestVersion);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override async Task<string []> GetVersions()
        {
            try
            {
                return GetEdgeVersion(await Client.DownloadStringTaskAsync("https://developer.microsoft.com/en-us/microsoft-edge/tools/webdriver/"))
                    .Select(m => Sanatize(m))
                    .ToArray();
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
                return Sanatize(await Client.DownloadStringTaskAsync("https://msedgedriver.azureedge.net/LATEST_STABLE"));
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
                string url = $"https://msedgedriver.azureedge.net/{version}/{fullName}";
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
               SystemType.WIN64,
               SystemType.MAC64,
               SystemType.LINUX64,
               SystemType.ARM64,
            };
        }

        public override void Dispose()
        {
            Client.Dispose();
        }

        private string [] GetEdgeVersion(string html)
        {
            Regex stringHtml = new Regex(@"Version: *(\d+\.{1})+\d+");
            var matches = stringHtml.Matches(html)
                .OrderByDescending(m => m.Value)
                .Select(m => m.Value)
                .ToList();

            if (matches.Count == 0)
                throw new Exception("Could not found the driver version");
            return matches.Select(m => m.Replace("Version: ","")).ToArray();
        }
    }
}
