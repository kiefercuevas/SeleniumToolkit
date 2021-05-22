﻿using OpenQA.Selenium;
using SeleniumToolkit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace SeleniumToolkit.TableElements
{
    public class TableElement : ITableElement, IDisposable
    {
        private readonly string _tagName = "table";
        private readonly string _anyWhereInChild = ".//";
        private readonly string _directChild = "./";
        private IList<IDictionary<string, TableCell>> _dictTable;
        private int DefaultTextIndex = 1;
        private string DefaultHeaderName = "DefaultHeader";

        private XpathExpression trExpression1;
        private XpathExpression trEXpression2;
        private string tdOrTh;
        public IList<TableCell> Headers { get; private set; }
        
        public int Count
        {
            get { return _dictTable.Count; }
        }

        public TableElement()
        {

        }

        /// <summary>
        /// Loads all the information from IWebElement table
        /// </summary>
        /// <param name="table">A IWebElement that represents a table</param>
        /// <param name="token">A cancelation token to cancel the operation</param>
        public void Load(IWebElement table, CancellationToken? token = null)
        {
            ValidateTable(table);
            SetParams();

            TableOptions options = new TableOptions();
            SetHeadersFromWebTable(table, token, options);
            SetBodyFromWebTable(table, token, options);
        }

        /// <summary>
        /// Loads all the information from IWebElement table
        /// </summary>
        /// <param name="table">A IWebElement that represents a table</param>
        /// <param name="options">provide some options into the Table element</param>
        /// <param name="token">A cancelation token to cancel the operation</param>
        public void Load(IWebElement table, TableOptions options, CancellationToken? token = null)
        {
            ValidateTable(table);

            if (options == null)
                throw new ArgumentNullException(nameof(options));

            SetParams();
            SetHeadersFromWebTable(table, token, options);
            SetBodyFromWebTable(table, token, options);
        }

        /// <summary>
        /// Append a new table body to the current table
        /// </summary>
        /// <param name="table">The new table to add</param>
        /// <param name="token">A cancelation token to cancel the operation</param>
        public void AddTableBody(IWebElement table, CancellationToken? token)
        {
            if (table.TagName.ToLower() != _tagName)
                throw new Exception(string.Format("The {0} is not a table", table.TagName));

            SetBodyFromWebTable(table, token, new TableOptions());
        }

        /// <summary>
        ///  Append a new table body to the current table
        /// </summary>
        /// <param name="table">The new table to add</param>
        /// <param name="options">provide some options into the Table element</param>
        /// <param name="token">A cancelation token to cancel the operation</param>
        public void AddTableBody(IWebElement table, TableOptions options, CancellationToken? token)
        {
            if (table.TagName.ToLower() != _tagName)
                throw new Exception(string.Format("The {0} is not a table", table.TagName));

            SetBodyFromWebTable(table, token, options);
        }

        /// <summary>
        /// Ge the column with the specified name
        /// </summary>
        /// <param name="column">Column name to look for</param>
        public IEnumerable<TableCell> GetColumn(string column)
        {
            ValidateColumn(column);
            return _dictTable.Select(r => r[column]).ToList();
        }

        /// <summary>
        /// Get the column with the specified column index
        /// </summary>
        /// <param name="column">Column index to look for</param>
        public IEnumerable<TableCell> GetColumn(int column)
        {
            ValidateColumn(column);
            return _dictTable.Select(r => r[Headers[column].Text]).ToList();
        }

        /// <summary>
        /// Get a row with the specified rowIndex
        /// </summary>
        /// <param name="rowIndex">The row Index to look for</param>
        public IDictionary<string, TableCell> GetRow(int rowIndex)
        {
            ValidateRowIndex(rowIndex);
            return _dictTable[rowIndex];
        }

        /// <summary>
        /// Get a row with the specified predicate
        /// </summary>
        /// <param name="rowIndex">The row predicate to use</param>
        public IDictionary<string, TableCell> GetRow(Func<IDictionary<string, TableCell>, bool> predicate)
        {
            return _dictTable.FirstOrDefault(predicate);
        }
        /// <summary>
        /// Get the last row of the current table
        /// </summary>
        /// <returns>A dictionary that represent the last row</returns>
        public IDictionary<string, TableCell> GetLastRow()
        {
            return _dictTable[_dictTable.Count - 1];
        }

        /// <summary>
        /// Get the first row of the current table
        /// </summary>
        /// <returns>A dictionary that represent the first row</returns>
        public IDictionary<string, TableCell> GetFirstRow()
        {
            return _dictTable.Count > 0 ? _dictTable[0] : null;
        }

        /// <summary>
        /// Return all rows that satisfy the criteria
        /// </summary>
        /// <param name="predicate">The criteria to look for</param>
        public IEnumerable<IDictionary<string, TableCell>> GetRows(Func<IDictionary<string, TableCell>, bool> predicate)
        {
            return _dictTable.Where(predicate).ToList();
        }

        /// <summary>
        /// Return all rows from the start to end index
        /// </summary>
        /// <param name="start">The start index to look for</param>
        /// <param name="end">The end index to look for</param>
        public IEnumerable<IDictionary<string, TableCell>> GetRows(int start, int end)
        {
            ValidateRowIndex(start);
            ValidateRowIndex(end);
            return _dictTable.Skip(start).Take(end).ToList();
        }

        /// <summary>
        /// Return all rows
        /// </summary>
        public IEnumerable<IDictionary<string, TableCell>> GetRows()
        {
            return _dictTable.ToList();
        }

        /// <summary>
        /// Return the amount of rows specified
        /// </summary>
        public IEnumerable<IDictionary<string, TableCell>> GetRows(int rowNumber)
        {
            ValidateRowIndex(rowNumber);
            return _dictTable.Take(rowNumber).ToList();
        }
        
        /// <summary>
        /// Removes a row at a given index
        /// </summary>
        /// <param name="rowIndex">The index of the row to remove</param>
        /// <returns>The row that was removed</returns>
        public IDictionary<string, TableCell> RemoveRow(int rowIndex)
        {
            var item = _dictTable[rowIndex];
            _dictTable.Remove(item);
            return item;
        }

        /// <summary>
        /// Removes the last row
        /// </summary>
        /// <returns>The row that was removed</returns>
        public IDictionary<string, TableCell> RemoveLastRow()
        {
            var item = _dictTable[Count - 1];
            _dictTable.Remove(item);
            return item;
        }

        /// <summary>
        /// Removes the first row
        /// </summary>
        /// <returns>The row that was removed</returns>
        public IDictionary<string, TableCell> RemoveFirstRow()
        {
            var item = _dictTable[Count - 1];
            _dictTable.Remove(item);
            return item;
        }
        
        /// <summary>
        /// Remove all columns that has the DefaultHeader Text
        /// </summary>
        public void RemoveDefaultColumns()
        {
            var headersToRemove = Headers.Where(h => h.Text.Contains(DefaultHeaderName)).ToList();
            var rows = GetRows();

            foreach (var header in headersToRemove)
            {
                Headers.Remove(header);
                foreach (IDictionary<string,TableCell> row in rows)
                    row.Remove(header.Text);
            }
                
        }

        /// <summary>
        /// Get all headers of the table
        /// </summary>
        public IEnumerable<string> GetHeaders()
        {
            return Headers.Select(h => h.Text).ToList();
        }

        /// <summary>
        /// Get all headers elements of the table
        /// </summary>
        public IEnumerable<IWebElement> GetHeaderElements()
        {
            return Headers.Select(h => h.Element).ToList();
        }

        /// <summary>
        /// Get all headers of the table as cell elements
        /// </summary>
        public IEnumerable<TableCell> GetHeaderCell()
        {
            return Headers.ToList();
        }

        /// <summary>
        /// Get all headers of the table as a dict where key is the header text and value is the element
        /// </summary>
        public IDictionary<string, IWebElement> GetHeadersDict()
        {
            IDictionary<string, IWebElement> dict = new Dictionary<string, IWebElement>();
            int index = 0;
            foreach (TableCell item in Headers)
            {
                if (dict.ContainsKey(item.Text))
                    dict.Add(item.Text + index.ToString(), item.Element);
                else
                    dict.Add(item.Text, item.Element);

                index++;
            }

            return dict;
        }

        /// <summary>
        /// Get the header web element
        /// </summary>
        /// <param name="columnName">The name of the column</param>
        /// <returns>The IWebElement that represent the column</returns>
        public IWebElement GetHeader(string columnName)
        {
            ValidateColumn(columnName);
            return Headers.FirstOrDefault(c => c.Text == columnName).Element;
        }

        /// <summary>
        /// Get the cell at the specified row index and column index, with the cellIndex param
        /// </summary>
        /// <param name="row">The row index to look for</param>
        /// <param name="column">The column index to look for</param>
        public TableCell GetCell(int row, int column)
        {
            ValidateRowIndex(row);
            ValidateColumn(column);
            return _dictTable[row][Headers[column].Text];
        }

        /// <summary>
        /// Get the cell at the specified row index and column index, with the cellIndex param
        /// </summary>
        /// <param name="row">The row index to look for</param>
        /// <param name="column">The column name to look for</param>
        public TableCell GetCell(int row, string column)
        {
            ValidateRowIndex(row);
            ValidateColumn(column);
            return _dictTable[row][column];
        }

        /// <summary>
        /// Validates if the table is empty
        /// </summary>
        /// <returns>true if the table is empty, otherwise false</returns>
        public bool IsEmpty()
        {
            return _dictTable.Count == 0;
        }

        /// <summary>
        /// Clear the current table data
        /// </summary>
        public void Clear()
        {
            Headers.Clear();
            _dictTable.Clear();
        }

        public void Dispose()
        {
            Clear();
        }

        private TableCell GetTd(IWebElement element, bool isFromHeader = false, string additionalText = "")
        {
            if (string.IsNullOrWhiteSpace(element.Text) && isFromHeader)
                return new TableCell(element, $"{DefaultHeaderName}{DefaultTextIndex++}");

            return new TableCell(element, element.Text + additionalText);
        }

        private void ValidateTable(IWebElement table)
        {
            if (table.TagName.ToLower() != _tagName)
                throw new Exception(string.Format("The {0} is not a table", table.TagName));
        }
        private void ValidateRowIndex(int row)
        {
            if (row >= _dictTable.Count || row < 0)
                throw new ArgumentOutOfRangeException();
        }
        private void ValidateColumn(string column)
        {
            bool hasHeader = false;
            for (int i = 0; i < Headers.Count; i++)
            {
                if (Headers[i].Text.Equals(column, StringComparison.InvariantCultureIgnoreCase))
                {
                    hasHeader = true;
                    break;
                }
            }

            if (!hasHeader)
                throw new ArgumentException("The column name is not valid");
        }
        private void ValidateColumn(int column)
        {
            if (column >= Headers.Count || column < 0)
                throw new ArgumentOutOfRangeException();
        }

        private void SetBodyFromWebTable(IWebElement table, CancellationToken? token, TableOptions options)
        {
            if (options.OnlyHeaders)
                return;

            ReadOnlyCollection<IWebElement> trs = table.FindElements(By.XPath(trExpression1.GetExpression()));
            if (trs.Count == 0)
                trs = table.FindElements(By.XPath(trEXpression2.GetExpression()));

            int startRow = options.StartRow > 0 ? options.StartRow : 0;
            int rowAmount = options.RowAmount > 0 ? options.RowAmount : trs.Count;

            //For each row in the table
            foreach (IWebElement row in trs.Skip(startRow).ToList())
            {
                if (IsCancelationRequested(token))
                {
                    if (options.ClearRowsIfOperationCancel)
                        Clear();

                    break;
                }

                ReadOnlyCollection<IWebElement> cells = row.FindElements(By.XPath(tdOrTh));
                if (cells.Count != Headers.Count)
                    continue;

                int headerIndex = 0;
                Dictionary<string, TableCell> currentRow = new Dictionary<string, TableCell>();
                foreach (IWebElement cell in cells)
                {
                    if (IsCancelationRequested(token))
                    {
                        if (options.ClearRowsIfOperationCancel)
                            Clear();

                        break;
                    }

                    currentRow.Add(Headers[headerIndex].Text, GetTd(cell));
                    headerIndex++;
                }

                OnRowCreated(currentRow);

                _dictTable.Add(currentRow);
                if (_dictTable.Count == rowAmount)
                    break;
            }
        }
        private void SetHeadersFromWebTable(IWebElement table, CancellationToken? token, TableOptions options)
        {
            string trXpression = new XpathExpression("tr", _anyWhereInChild).WherePosition(1).GetExpression();
            IWebElement header = table.FindElement(By.XPath(trXpression));
            ReadOnlyCollection<IWebElement> tds = header.FindElements(By.XPath(tdOrTh));
            
            HashSet<string> tempHeaders = new HashSet<string>();
            Headers = new List<TableCell>();

            for (int i = 0; i < tds.Count; i++)
            {
                if (IsCancelationRequested(token))
                {
                    if (options.ClearRowsIfOperationCancel)
                        Clear();

                    break;
                }

                TableCell cell = GetTd(tds[i], 
                                    isFromHeader: true, 
                                    additionalText: tempHeaders.Contains(tds[i].Text) ? i.ToString() : "");

                tempHeaders.Add(cell.Text);
                Headers.Add(cell);
            }
        }
        private void SetParams()
        {
            _dictTable = new List<IDictionary<string, TableCell>>();
            trExpression1 = new XpathExpression("tr", _anyWhereInChild).WherePositionGreaterThan(1).WhereAncestor(new XpathExpression("table", _anyWhereInChild).WhereNotDescendant("thead"));
            trEXpression2 = new XpathExpression("tr", _anyWhereInChild).WherePositionGreaterOrEqualThan(1).WhereNotParent("thead");
            tdOrTh = new XpathExpression("th", _directChild).Union(new XpathExpression("td", _directChild));
        }

        private bool IsCancelationRequested(CancellationToken? token)
        {
            if (token.HasValue)
                return token.Value.IsCancellationRequested;
            return false;
        }


        public event EventHandler<IReadOnlyDictionary<string, TableCell>> RowCreated;
        protected virtual void OnRowCreated(IReadOnlyDictionary<string, TableCell> row)
        {
            RowCreated?.Invoke(this, row);
        }

    }
}
