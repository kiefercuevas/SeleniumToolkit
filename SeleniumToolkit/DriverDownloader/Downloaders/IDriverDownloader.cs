using System;
using System.Threading.Tasks;

namespace SeleniumToolkit.DriverDownloader.Downloaders
{
    /// <summary>
    /// An interface that provide download of driver files
    /// </summary>
    public interface IDriverDownloader :IDisposable
    {
        /// <summary>Gets the partial name of the driver file</summary>
        string PartialName { get; }
        /// <summary>All supported system types</summary>
        /// <returns>All supported system types of this instance</returns>
        SystemType[] GetSystemTypes();
        /// <summary>
        /// Download the driver file
        /// </summary>
        /// <param name="version">The version to download</param>
        /// <param name="directoryPath">The directory to store the file</param>
        /// <param name="system">The type of file system</param>
        /// <returns>The path of the driver file</returns>
        Task<string> DownLoad(string version, string directoryPath, SystemType system);
        /// <summary>
        /// Get the latest stable version of the driver
        /// </summary>
        /// <returns>A string that represents the latest stable version of the driver</returns>
        Task<string> GetLastestVersion();
    }
}