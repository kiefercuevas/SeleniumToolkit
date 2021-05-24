using OpenQA.Selenium;

namespace SeleniumToolkit.TableElements
{
    /// <summary>
    /// A class to represent a cell in TableElement class
    /// </summary>
    public class TableCell
    {
        /// <summary>
        /// Web element of the cell
        /// </summary>
        public readonly IWebElement Element;
        /// <summary>
        /// Text of the web element
        /// </summary>
        public readonly string Text;

        public TableCell(IWebElement element, string text)
        {
            Element = element;
            Text = text;
        }

        public override string ToString()
        {
            return $"{Text}";
        }
    }
}
