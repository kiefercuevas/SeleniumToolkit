using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumToolkit.DriverDownloader.Downloaders
{
    public enum SystemType
    {
        /// <summary>File type valid for Edge browser</summary>
        ARM64,
        /// <summary>File type valid for Edge, Mozilla, Chrome and Opera browser</summary>
        WIN32,
        /// <summary>File type valid for Edge, Mozilla and Opera browser</summary>
        WIN64,
        /// <summary>File type valid for Mozilla browser</summary>
        LINUX32_TAR_GZ,
        /// <summary>File type valid for Mozilla browser</summary>
        LINUX32_TAR_GZ_ASC,
        /// <summary>File type valid for Chrome, Mozilla and Opera browser</summary>
        LINUX64,
        /// <summary>File type valid for Mozilla browser</summary>
        LINUX64_TAR_GZ,
        /// <summary>File type valid for Mozilla browser</summary>
        LINUX64_TAR_GZ_ASC,
        /// <summary>File type valid for Chrome and Opera browser</summary>
        MAC64,
        /// <summary>File type valid for Chrome</summary>
        MAC64_M1,
        /// <summary>File type valid for Mozilla browser</summary>
        MACOS,
        /// <summary>File type valid for Mozilla browser</summary>
        MACOS_AARCH64,
    }
}
