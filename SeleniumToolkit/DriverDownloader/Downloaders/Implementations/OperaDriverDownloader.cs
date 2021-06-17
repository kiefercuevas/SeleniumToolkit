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
    public class OperaDriverDownloader :DriverDownloader
    {
        private readonly WebClient Client;
        public OperaDriverDownloader()
            : base("operadriver")
        {
            Client = new WebClient();
        }

        public override async Task<string> GetLastestVersion()
        {
            try
            {
                return Sanatize(GetOperaVersion(await Client.DownloadStringTaskAsync("https://github.com/operasoftware/operachromiumdriver/releases/latest")));
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
                return GetOperaVersions(await Client.DownloadStringTaskAsync("https://github.com/operasoftware/operachromiumdriver/releases"))
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
            return await GetLastestVersion();
        }

        public override async Task<string> DownLoad(string version, string directoryPath, SystemType system)
        {
            try
            {
                ValidateSystemType(system);

                string fullName = $"{PartialName}_{GetDriverTypeName(system)}";
                string url = $"https://github.com/operasoftware/operachromiumdriver/releases/download/{version}/{fullName}";
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
               SystemType.LINUX64
            };
        }

        public override void Dispose()
        {
            Client.Dispose();
        }

        private string GetOperaVersion(string html)
        {
            Regex stringHtml = new Regex(@"https://github.com/operasoftware/operachromiumdriver/releases/tag/(v|V){1}.?(\d+\.{1})+\d+");
            Match match = stringHtml.Match(html);

            return string.IsNullOrWhiteSpace(match.Value) ? string.Empty : Path.GetFileName(match.Value);
        }

        private string [] GetOperaVersions(string html)
        {
            Regex stringHtml = new Regex(@"operachromiumdriver/releases/tag/(v|V){1}.?(\d+\.{1})+\d+");
            MatchCollection matches = stringHtml.Matches(html);

            return matches.Where(m => string.IsNullOrWhiteSpace(m.Value) == false)
                .Select(m => Path.GetFileName(m.Value))
                .ToArray();
        }
    }
}
