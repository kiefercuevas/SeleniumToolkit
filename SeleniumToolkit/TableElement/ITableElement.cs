using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Threading;

namespace SeleniumToolkit.TableElements
{
    public interface ITableElement
    {
        int Count { get; }
        IList<TableCell> Headers { get; }

        event EventHandler<IReadOnlyDictionary<string, TableCell>> RowCreated;

        void AddTableBody(IWebElement table, CancellationToken? token);
        void AddTableBody(IWebElement table, TableOptions options, CancellationToken? token);
        void Clear();
        void Dispose();
        TableCell GetCell(int row, int column);
        TableCell GetCell(int row, string column);
        IEnumerable<TableCell> GetColumn(int column);
        IEnumerable<TableCell> GetColumn(string column);
        IDictionary<string, TableCell> GetFirstRow();
        IWebElement GetHeader(string columnName);
        IEnumerable<TableCell> GetHeaderCell();
        IEnumerable<IWebElement> GetHeaderElements();
        IEnumerable<string> GetHeaders();
        IDictionary<string, IWebElement> GetHeadersDict();
        IDictionary<string, TableCell> GetLastRow();
        IDictionary<string, TableCell> GetRow(Func<IDictionary<string, TableCell>, bool> predicate);
        IDictionary<string, TableCell> GetRow(int rowIndex);
        IDictionary<string, TableCell> RemoveRow(int rowIndex);
        IDictionary<string, TableCell> RemoveLastRow();
        IDictionary<string, TableCell> RemoveFirstRow();
        void RemoveDefaultColumns();
        IEnumerable<IDictionary<string, TableCell>> GetRows();
        IEnumerable<IDictionary<string, TableCell>> GetRows(Func<IDictionary<string, TableCell>, bool> predicate);
        IEnumerable<IDictionary<string, TableCell>> GetRows(int rowNumber);
        IEnumerable<IDictionary<string, TableCell>> GetRows(int start, int end);
        bool IsEmpty();
        void Load(IWebElement table, CancellationToken? token = null);
        void Load(IWebElement table, TableOptions options, CancellationToken? token = null);
    }
}