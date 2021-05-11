using OpenQA.Selenium;

namespace SeleniumToolkit.WebDriverProviders
{
    public interface IWebDriverProvider
    {
        int TimeOutInMinutes { get; }
        IWebDriver Driver { get; }
        void Open();
        void CloseDriver();
        void ResetDriver();
        IWebDriver GetDriverInstance();

    }
}
