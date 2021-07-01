using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Threading.Tasks;

namespace SeleniumToolkit.WebDriverProviders
{
    public class ChromeDriverProvider : IWebDriverProvider
    {
        public int TimeOutInMinutes { get; private set; }
        public IWebDriver Driver { get; private set; }
        public ChromeDriverProvider(int timeoutInMinutes = 5)
        {
            TimeOutInMinutes = timeoutInMinutes;
        }

        public void CloseDriver()
        {
            Driver.Quit();
            Driver = null;
        }

        public void ResetDriver()
        {
            Driver.Quit();
            Driver.Dispose();
            Driver = null;
            Driver = GetDriverInstance();
            Driver.Manage().Window.Maximize();
        }

        public void Open()
        {
            Driver = GetDriverInstance();
        }

        public virtual async Task OpenAsync()
        {
            Open();
        }

        public virtual async Task ResetAsyncDriver()
        {
            ResetDriver();
        }

        public IWebDriver GetDriverInstance()
        {
            string path = Environment.CurrentDirectory;
            return new ChromeDriver(path, options: new ChromeOptions(), commandTimeout: TimeSpan.FromMinutes(TimeOutInMinutes));
        }

    }
}
