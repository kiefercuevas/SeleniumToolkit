using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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
        
    }
}
