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

        /// <summary>
        /// Get the cell at the specified row index and column index, with the cellIndex param
        /// </summary>
        /// <param name="row">The row index to look for</param>
        /// <param name="column">The column index to look for</param>
        TableCell GetCell(int row, int column);
        /// <summary>
        /// Get the cell at the specified row index and column index, with the cellIndex param
        /// </summary>
        /// <param name="row">The row index to look for</param>
        /// <param name="column">The column name to look for</param>
        TableCell GetCell(int row, string column);

        /// <summary>
        /// Ge the column with the specified name
        /// </summary>
        /// <param name="column">Column name to look for</param>
        IEnumerable<TableCell> GetColumn(int column);
        /// <summary>
        /// Get the column with the specified column index
        /// </summary>
        /// <param name="column">Column index to look for</param>
        IEnumerable<TableCell> GetColumn(string column);
        /// <summary>
        /// Get all headers of the table as a dict where key is the header text and value is the element
        /// </summary>
        IDictionary<string, IWebElement> GetHeadersDict();
        /// <summary>
        /// Get the string value of each header cell
        /// </summary>
        IEnumerable<string> GetHeadersString();
        
        /// <summary>
        /// Get a row with the specified rowIndex
        /// </summary>
        /// <param name="rowIndex">The row Index to look for</param>
        IDictionary<string, TableCell> GetRow(int rowIndex);
        /// <summary>
        /// Get a row with the specified predicate
        /// </summary>
        /// <param name="predicate">The criteria to look for</param>
        IDictionary<string, TableCell> GetRow(Func<IDictionary<string, TableCell>, bool> predicate);
        /// <summary>
        /// Get the first row of the current table
        /// </summary>
        /// <returns>A dictionary that represent the first row</returns>
        IDictionary<string, TableCell> GetFirstRow();
        /// <summary>
        /// Get the last row of the current table
        /// </summary>
        /// <returns>A dictionary that represent the last row</returns>
        IDictionary<string, TableCell> GetLastRow();
        /// <summary>
        /// Return all rows
        /// </summary>
        IEnumerable<IDictionary<string, TableCell>> GetRows();
        /// <summary>
        /// Return the amount of rows specified
        /// </summary>
        IEnumerable<IDictionary<string, TableCell>> GetRows(int rowNumber);
        /// <summary>
        /// Return all rows from the start to end index
        /// </summary>
        /// <param name="start">The start index to look for</param>
        /// <param name="end">The end index to look for</param>
        IEnumerable<IDictionary<string, TableCell>> GetRows(int start, int end);
        /// <summary>
        /// Return all rows that satisfy the criteria
        /// </summary>
        /// <param name="predicate">The criteria to look for</param>
        IEnumerable<IDictionary<string, TableCell>> GetRows(Func<IDictionary<string, TableCell>, bool> predicate);
        
        /// <summary>
        /// Add a valid row to the table element
        /// </summary>
        /// <param name="row">The row to add to the table</param>
        void AddRow(IDictionary<string, TableCell> row);

        /// <summary>
        /// Removes a row at a given index
        /// </summary>
        /// <param name="rowIndex">The index of the row to remove</param>
        /// <returns>The row that was removed</returns>
        IDictionary<string, TableCell> RemoveRow(int rowIndex);
        /// <summary>
        /// Removes the first row
        /// </summary>
        /// <returns>The row that was removed</returns>
        IDictionary<string, TableCell> RemoveFirstRow();
        /// <summary>
        /// Removes the last row
        /// </summary>
        /// <returns>The row that was removed</returns>
        IDictionary<string, TableCell> RemoveLastRow();
        /// <summary>
        /// Remove all rows that satisfy the criteria
        /// </summary>
        /// <param name="predicate">The criteria to look for</param>
        void RemoveRows(Func<IDictionary<string, TableCell>, bool> predicate);
        /// <summary>
        /// Remove all columns that has the DefaultHeader Text
        /// </summary>
        void RemoveDefaultColumns();
        /// <summary>
        /// Validates if the table is empty
        /// </summary>
        /// <returns>true if the table is empty, otherwise false</returns>
        bool IsEmpty();

        /// <summary>
        /// Loads all the information from IWebElement table
        /// </summary>
        /// <param name="token">A cancelation token to cancel the operation</param>
        void Load(CancellationToken? token = null);
        /// <summary>
        /// Loads all the information from IWebElement table
        /// </summary>
        /// <param name="options">provide some options into the Table element</param>
        /// <param name="token">A cancelation token to cancel the operation</param>
        void Load(TableOptions options, CancellationToken? token = null);

        /// <summary>
        /// Clear the current table data
        /// </summary>
        void Clear();
        void Dispose();

        event EventHandler<IReadOnlyDictionary<string, TableCell>> RowCreated;
    }
}