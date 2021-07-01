using OpenQA.Selenium;
using System.Threading.Tasks;

namespace SeleniumToolkit.WebDriverProviders
{
    public interface IWebDriverProvider
    {
        int TimeOutInMinutes { get; }
        IWebDriver Driver { get; }
        void Open();
        void CloseDriver();
        void ResetDriver();
        Task OpenAsync();
        Task ResetDriverAsync();
        IWebDriver GetDriverInstance();

    }
}
