using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;

namespace SeleniumToolkit
{
    public class WindowTabManager
    {

        private readonly Dictionary<string, Tab> Tabs;
        private readonly IWebDriver Driver;
        private Tab MainTab;
        public int Count {get => Tabs.Count;}

        public WindowTabManager(IWebDriver driver)
        {
            Driver = driver;

            Tabs = new Dictionary<string, Tab>();
            MainTab = new Tab(0, driver.CurrentWindowHandle, driver.Title);

            Tabs.Add(MainTab.Handle, MainTab);
        }

        public void LookForNewhHandles()
        {
            foreach (string handle in Driver.WindowHandles)
            {
                if (!Tabs.ContainsKey(handle))
                {
                    Tabs.Add(handle, new Tab(Count, handle, string.Empty));
                    SetTabTitle(handle);
                }
            }
        }
        
        
        public string GoToMainTab()
        {
            foreach (Tab item in Tabs.Values)
            {
                if(item.Index == 0)
                {
                    Driver.SwitchTo().Window(item.Handle);
                    return item.Handle;
                }
            }
            return null;
        }

        public string GoTabByIndex(int index)
        {
            if (index < 0 || index > Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            string handle = GetByIndex(index);
            Driver.SwitchTo().Window(handle);
            return handle;
        }

        public void GoToUrl(string url)
        {
            Driver.Navigate().GoToUrl(url);
            SetTabTitle(Driver.CurrentWindowHandle);
        }

        public void GoToUrl(IEnumerable<string> urls, bool ignoreErrors = false)
        {
            if(urls.Count() > Tabs.Count)
            {
                throw new ArgumentOutOfRangeException("The are not enough tabs to navigate");
            }

            int index = 0;
            foreach (string item in urls)
            {
                try
                {
                    Driver.SwitchTo().Window(GetByIndex(index));
                    Driver.Navigate().GoToUrl(item);
                }
                catch (Exception)
                {
                    if (!ignoreErrors)
                    {
                        throw;
                    }
                }
                finally
                {
                    SetTabTitle(Driver.CurrentWindowHandle);
                    index++;
                }
            }
            
        }

        public void GoToUrl(string url, string handle)
        {
            Driver.SwitchTo().Window(handle);
            Driver.Navigate().GoToUrl(url);

            SetTabTitle(handle);
        }

        public void GoToUrl(string url, int tabIndex)
        {
            string handle = GetByIndex(tabIndex);
            Driver.SwitchTo().Window(handle);
            Driver.Navigate().GoToUrl(url);

            SetTabTitle(handle);
        }

        public void GoToUrlWithTabTitle(string url, string tabTitle)
        {
            string handle = GetByPageTitle(tabTitle);
            Driver.SwitchTo().Window(handle);
            Driver.Navigate().GoToUrl(url);

            SetTabTitle(handle);
        }


        public void Refresh()
        {
            Driver.Navigate().Refresh();
            SetTabTitle(Driver.CurrentWindowHandle);
        }

        public void Refresh(string handle)
        {
            Driver.SwitchTo().Window(handle);
            Driver.Navigate().Refresh();

            SetTabTitle(handle);
        }

        public void Refresh(int tabIndex)
        {
            string handle = GetByIndex(tabIndex);

            Driver.SwitchTo().Window(handle);
            Driver.Navigate().Refresh();

            SetTabTitle(handle);
        }

        public void RefreshWithTabTitle(string tabTitle)
        {
            string handle = GetByPageTitle(tabTitle);

            Driver.SwitchTo().Window(handle);
            Driver.Navigate().Refresh();

            SetTabTitle(handle);
        }


        public void Back()
        {
            Driver.Navigate().Back();
            SetTabTitle(Driver.CurrentWindowHandle);
        }

        public void Back(string handle)
        {
            Driver.SwitchTo().Window(handle);
            Driver.Navigate().Back();

            SetTabTitle(handle);
        }

        public void Back(int tabIndex)
        {
            string handle = GetByIndex(tabIndex);

            Driver.SwitchTo().Window(handle);
            Driver.Navigate().Back();

            SetTabTitle(handle);
        }

        public void BackWithTabTitle(string tabTitle)
        {
            string handle = GetByPageTitle(tabTitle);

            Driver.SwitchTo().Window(handle);
            Driver.Navigate().Back();

            SetTabTitle(handle);
        }


        public void Forward()
        {
            Driver.Navigate().Forward();
            SetTabTitle(Driver.CurrentWindowHandle);
        }

        public void Forward(string handle)
        {
            Driver.SwitchTo().Window(handle);
            Driver.Navigate().Forward();

            SetTabTitle(handle);
        }

        public void Forward(int tabIndex)
        {
            string handle = GetByIndex(tabIndex);

            Driver.SwitchTo().Window(handle);
            Driver.Navigate().Forward();

            SetTabTitle(handle);
        }

        public void ForwardWithTabTitle(string tabTitle)
        {
            string handle = GetByPageTitle(tabTitle);

            Driver.SwitchTo().Window(handle);
            Driver.Navigate().Forward();

            SetTabTitle(handle);
        }


        public string GetNewTabHandleFromDriver(IWebDriver driver)
        {
            foreach (string handle in driver.WindowHandles)
            {
                if (!Tabs.ContainsKey(handle))
                {
                    return handle;
                }
            }

            return null;
        }

        public string OpenTab(IJavaScriptExecutor driver)
        {
            driver.ExecuteScript("window.open();");

            Tab newHandleWindow = new Tab(Count, GetNewTabHandleFromDriver(Driver), Driver.Title);
            AddWindowHandle(newHandleWindow);

            Driver.SwitchTo().Window(newHandleWindow.Handle);
            return newHandleWindow.Handle;
        }

        public string CloseLastTab()
        {
            if(Tabs.Count == 0)
            {
                return string.Empty;
            }

            string handle = GetByIndex(Tabs.Count - 1);
            
            CloseTab(handle);

            return handle;
        }

        public void ClosePages()
        {
            int amount = Tabs.Count;
            for (int i = 0; i < amount; i++)
            {
                CloseLastTab();
            }
        }

        public void CloseTabsExceptMain()
        {
            if (Tabs.Count == 1)
                return;

            int amount = Tabs.Count - 1;
            for (int i = 0; i < amount; i++)
            {
                CloseLastTab();
            }

            Driver.SwitchTo().Window(MainTab.Handle);
        }

        public void CloseTab(string handle)
        {
            if (!Tabs.ContainsKey(handle))
            {
                throw new ArgumentException("The handle does not exist");
            }

            Driver.SwitchTo().Window(handle);
            Driver.Close();

            int deleteTabIndex = Tabs[handle].Index;
            Tabs.Remove(handle);

            foreach (Tab item in Tabs.Values)
            {
                if (item.Index > deleteTabIndex)
                {
                    item.Index--;
                }
            }

            MainTab = Tabs.Values.FirstOrDefault(t => t.Index == 0);
        }

        public List<string> OpenPages(IEnumerable<string> urls)
        {
            List<string> handles = new List<string>();
            foreach (string item in urls)
            {
                handles.Add(OpenTab((IJavaScriptExecutor)Driver));
                GoToUrl(item);
            }
            return handles;
        }

        public string OpenPage(string url)
        {
            string newHandle = OpenTab((IJavaScriptExecutor)Driver);
            GoToUrl(url);

            return newHandle;
        }

        public List<string> OpenPages(IJavaScriptExecutor driver, IEnumerable<string> urls)
        {
            List<string> handles = new List<string>();
            foreach (string item in urls)
            {
                handles.Add(OpenTab(driver));
                GoToUrl(item);
            }
            return handles;
        }

        public void OpenTab(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                OpenTab((IJavaScriptExecutor)Driver);
            }
        }

        public void OpenTab(IJavaScriptExecutor driver, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                OpenTab(driver);
            }
        }

        public string GetByPageTitle(string title)
        {
            foreach (Tab item in Tabs.Values)
            {
                if (item.LastTitle.ToLower().Contains(title.ToLower()))
                {
                    Driver.SwitchTo().Window(item.Handle);
                    return item.Handle;
                }
            }

            return null;
        }

        public string GetByIndex(int index)
        {
            if (index < 0 || index > Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            foreach (Tab item in Tabs.Values)
            {
                if (item.Index == index)
                {
                    Driver.SwitchTo().Window(item.Handle);
                    return item.Handle;
                }
            }

            return null;
        }

        public string GetTitle(int index)
        {
            if (index < 0 || index > Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            foreach (Tab item in Tabs.Values)
            {
                if (item.Index == index)
                {
                    Driver.SwitchTo().Window(item.Handle);
                    return item.LastTitle;
                }
            }

            return null;
        }

        public string GetBody(int index)
        {
            if (index < 0 || index > Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            foreach (Tab item in Tabs.Values)
            {
                if (item.Index == index)
                {
                    Driver.SwitchTo().Window(item.Handle);
                    return Driver.FindElement(By.TagName("body")).Text;
                }
            }

            return null;
        }

        public bool SetTabTitle(string tabHandle)
        {
            if (Tabs.ContainsKey(tabHandle))
            {
                Tabs[tabHandle].LastTitle = Driver.Title;
            }
            return false;
        }

        public bool SetTabTitle(string tabHandle, string newTitle)
        {
            if (Tabs.ContainsKey(tabHandle))
            {
                Tabs[tabHandle].LastTitle = newTitle;
            }
            return false;
        }

        public bool SetTabTitle(string tabHandle, IWebDriver driver)
        {
            if (Tabs.ContainsKey(tabHandle))
            {
                Tabs[tabHandle].LastTitle = driver.Title;
            }
            return false;
        }

        

        private void AddWindowHandle(Tab window)
        {
            Tabs.Add(window.Handle, window);
        }

        private class Tab
        {
            public int Index;
            public readonly string Handle;
            public string LastTitle;

            public Tab()
            {

            }

            public Tab(int index, string handle, string title)
            {
                Index = index;
                Handle = handle;
                LastTitle = title;
            }

            public override string ToString()
            {
                return LastTitle;
            }

        }
    }

}
