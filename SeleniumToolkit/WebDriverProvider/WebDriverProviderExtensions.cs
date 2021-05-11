using OpenQA.Selenium;
using System;
using System.Threading.Tasks;

namespace SeleniumToolkit.WebDriverProviders.Extensions
{
    public static class WebDriverProviderExtensions
    {
        public static async Task<IWebElement> LookForElementByCallBack(this IWebDriverProvider provider, 
            By value, int attemps = 3, int intervalForAttemp = 2000)
        {
            int counter = 0;
            while (counter < attemps)
            {
                try
                {
                    IWebElement element = provider.Driver.FindElement(value);
                    if (element != null && element.Enabled && element.Displayed)
                        return element;
                }
                catch (Exception) { }

                await Task.Delay(intervalForAttemp);
                counter++;
            }

            return null;
        }

        public static async Task<IWebElement> LookForElementByCallBack(this IWebDriverProvider provider,
            Func<By, IWebElement> callback, By value, int attemps = 3, int intervalForAttemp = 2000)
        {
            int counter = 0;
            while (counter < attemps)
            {
                try
                {
                    IWebElement element = callback(value);
                    if (element != null && element.Enabled && element.Displayed)
                        return element;
                }
                catch (Exception) { }

                await Task.Delay(intervalForAttemp);
                counter++;
            }

            return null;
        }

        public static IAlert LookForAlert(this IWebDriverProvider provider)
        {
            try
            {
                var alert = provider.Driver.SwitchTo().Alert();
                return alert;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
