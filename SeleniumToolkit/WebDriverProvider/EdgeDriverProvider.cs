using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumToolkit.WebDriverProviders
{
    public class EdgeDriverProvider : IWebDriverProvider
    {
        public int TimeOutInMinutes { get; private set; }
        public IWebDriver Driver { get; private set; }
        public EdgeDriverProvider(int timeoutInMinutes = 5)
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

        public virtual async Task ResetDriverAsync()
        {
            ResetDriver();
        }

        public IWebDriver GetDriverInstance()
        {
            string path = Environment.CurrentDirectory;
            return new EdgeDriver(path, options: new EdgeOptions(), commandTimeout: TimeSpan.FromMinutes(TimeOutInMinutes));
        }
    }
}
