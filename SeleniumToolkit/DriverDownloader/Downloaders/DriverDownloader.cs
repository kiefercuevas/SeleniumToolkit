using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumToolkit.DriverDownloader.Downloaders
{

    public abstract class DriverDownloader : IDriverDownloader
    {
        public string PartialName { get; private set; }

        /// <summary>
        /// Convert the enumeration of systemType into a partial string that represents the system
        /// </summary>
        /// <param name="systemType">The type of system to look for</param>
        /// <returns>An string that represents the file system type</returns>
        protected string GetDriverTypeName(SystemType systemType)
        {
            return systemType switch
            {
                SystemType.WIN32 => "win32.zip",
                SystemType.WIN64 => "win64.zip",
                SystemType.MAC64 => "mac64.zip",
                SystemType.LINUX64 => "linux64.zip",
                SystemType.ARM64 => "arm64.zip",
                SystemType.LINUX32_TAR_GZ => "linux32.tar.gz",
                SystemType.LINUX32_TAR_GZ_ASC => "linux32.tar.gz.asc",
                SystemType.LINUX64_TAR_GZ => "linux64.tar.gz",
                SystemType.LINUX64_TAR_GZ_ASC => "linux64.tar.gz.asc",
                SystemType.MACOS => "macos.tar.gz",
                SystemType.MACOS_AARCH64 => "macos-aarch64.tar.gz",
                SystemType.MAC64_M1 => "mac64_m1.zip",
                _ => throw new Exception("The System type is not valid"),
            };
        }

        protected void ValidateSystemType(SystemType type)
        {
            SystemType [] systemTypes = GetSystemTypes();
            if (systemTypes.Contains(type) == false)
                throw new Exception($"The {type} type is not valid for this driver, *valid types: {string.Join('|', systemTypes)}*");
        }

        protected string Sanatize(string value)
        {
            return value.Replace("\n", string.Empty)
                        .Replace("\r", string.Empty)
                        .Replace("&", string.Empty);
        }

        public abstract SystemType[] GetSystemTypes();

        public abstract Task<string> DownLoad(string version, string directoryPath, SystemType system);
        public abstract Task<string> GetLastestVersion();
        public abstract Task<string> GetStableVersion();
        public abstract void Dispose();

        public DriverDownloader(string partialName)
        {
            PartialName = partialName;
        }
    }
}
