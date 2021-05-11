using OpenQA.Selenium;

namespace SeleniumToolkit.TableElements
{
    /// <summary>
    /// A class to represent a cell in TableElement class
    /// </summary>
    public class Cell
    {
        /// <summary>
        /// Web element of the cell
        /// </summary>
        public IWebElement Element { get; set; }
        /// <summary>
        /// Text of the web element
        /// </summary>
        public string Text { get; set; }
        public override string ToString()
        {
            return $"{Text}";
        }
    }
}
