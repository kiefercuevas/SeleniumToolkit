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
    public class MozillaDriverDownloader : DriverDownloader
    {
        private readonly WebClient Client;
        public MozillaDriverDownloader()
            : base("geckodriver")
        {
            Client = new WebClient();
        }

        public override async Task<string> GetLastestVersion()
        {
            try
            {
                return Sanatize(GetMozillaVersion(await Client.DownloadStringTaskAsync("https://github.com/mozilla/geckodriver/releases/latest")));
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

                string fullName = $"{PartialName}-{version}-{GetDriverTypeName(system)}";
                string url = $"https://github.com/mozilla/geckodriver/releases/download/{version}/{fullName}";
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
               SystemType.MACOS,
               SystemType.MACOS_AARCH64,
               SystemType.LINUX32_TAR_GZ,
               SystemType.LINUX32_TAR_GZ_ASC,
               SystemType.LINUX64_TAR_GZ,
               SystemType.LINUX64_TAR_GZ_ASC
            };
        }

        public override void Dispose()
        {
            Client.Dispose();
        }

        private string GetMozillaVersion(string html)
        {
            Regex stringHtml = new Regex(@"https://github.com/mozilla/geckodriver/releases/tag/(v|V){1}.?(\d+.?)+");
            Match match = stringHtml.Match(html);

            return string.IsNullOrWhiteSpace(match.Value) ? string.Empty : Path.GetFileName(match.Value);
        }

    }
}
