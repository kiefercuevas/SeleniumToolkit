using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumToolkit.DriverDownloader.Downloaders.Extensions
{
    /// <summary>
    /// A class that provides the names of all the driver implementations
    /// </summary>
    public static class DriverNames
    {
        /// <summary>Name use for the EdgeDriver class in selenium</summary>
        public const string Edge = "MicrosoftWebDriver";
        /// <summary>Name use for the ChromeDriver class in selenium</summary>
        public const string Chrome = "chromedriver";
        /// <summary>Name use for the OperaDriver class in selenium</summary>
        public const string Opera = "operadriver";
        /// <summary>Name use for the MozillaDriver class in selenium</summary>
        public const string Mozilla = "geckodriver";
    }
}
