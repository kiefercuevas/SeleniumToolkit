using OpenQA.Selenium;
using System;
using System.Collections.Generic;

namespace SeleniumToolkit.TableElements
{
    public interface ITableElement
    {
        int Count { get; }
        IList<Cell> Headers { get; }

        void AddTable(IWebElement table);
        void AddTable(IWebElement table, int startRow, int amountOfRows);
        void Clear();
        void Dispose();
        Cell GetCell(int row, int column);
        Cell GetCell(int row, string column);
        IEnumerable<Cell> GetColumn(int column);
        IEnumerable<Cell> GetColumn(string column);
        IDictionary<string, Cell> GetFirstRow();
        IWebElement GetHeader(string columnName);
        IEnumerable<Cell> GetHeaderCell();
        IEnumerable<IWebElement> GetHeaderElements();
        IEnumerable<string> GetHeaders();
        IDictionary<string, IWebElement> GetHeadersDict();
        IDictionary<string, Cell> GetLastRow();
        IDictionary<string, Cell> GetRow(Func<IDictionary<string, Cell>, bool> predicate);
        IDictionary<string, Cell> GetRow(int rowIndex);
        IEnumerable<IDictionary<string, Cell>> GetRows();
        IEnumerable<IDictionary<string, Cell>> GetRows(Func<IDictionary<string, Cell>, bool> predicate);
        IEnumerable<IDictionary<string, Cell>> GetRows(int rowNumber);
        IEnumerable<IDictionary<string, Cell>> GetRows(int start, int end);
        bool IsEmpty();
    }
}