using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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

        public override void Dispose()
        {
            Client.Dispose();
        }
    }
}
