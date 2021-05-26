using OpenQA.Selenium;
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
        private int _defaultTextIndex = 1;
        private readonly string _defaultHeaderName = "DefaultHeader";
        private readonly IWebElement _tableElement;

        private IList<IDictionary<string, TableCell>> _dictTable;
        private string tdOrTh;

        public int Count{ get { return _dictTable.Count; } }
        public IList<TableCell> Headers { get; private set; }
        
        /// <summary>
        /// Creates a new tableElement with the specified table
        /// </summary>
        ///<param name="table">A IWebElement that represents a table</param>
        public TableElement(IWebElement table)
        {
            ValidateTable(table);
            _tableElement = table;

            SetParams();
        }

        public TableCell GetCell(int row, int column)
        {
            ValidateRowIndex(row);
            ValidateColumn(column);
            return _dictTable[row][Headers[column].Text];
        }
        public TableCell GetCell(int row, string column)
        {
            ValidateRowIndex(row);
            ValidateColumn(column);
            return _dictTable[row][column];
        }

        public IEnumerable<TableCell> GetColumn(string column)
        {
            ValidateColumn(column);
            return _dictTable.Select(r => r[column]).ToList();
        }
        public IEnumerable<TableCell> GetColumn(int column)
        {
            ValidateColumn(column);
            return _dictTable.Select(r => r[Headers[column].Text]).ToList();
        }

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
        public IEnumerable<string> GetHeadersString()
        {
            return Headers.Select(h => h.Text).ToList();
        }

        public IDictionary<string, TableCell> GetRow(int rowIndex)
        {
            ValidateRowIndex(rowIndex);
            return _dictTable[rowIndex];
        }
        public IDictionary<string, TableCell> GetRow(Func<IDictionary<string, TableCell>, bool> predicate)
        {
            return _dictTable.FirstOrDefault(predicate);
        }
        public IDictionary<string, TableCell> GetFirstRow()
        {
            return _dictTable.Count > 0 ? _dictTable[0] : null;
        }
        public IDictionary<string, TableCell> GetLastRow()
        {
            return _dictTable[_dictTable.Count - 1];
        }

        public IEnumerable<IDictionary<string, TableCell>> GetRows()
        {
            return _dictTable.ToList();
        }
        public IEnumerable<IDictionary<string, TableCell>> GetRows(int rowNumber)
        {
            ValidateRowIndex(rowNumber);
            return _dictTable.Take(rowNumber).ToList();
        }
        public IEnumerable<IDictionary<string, TableCell>> GetRows(int start, int end)
        {
            ValidateRowIndex(start);
            ValidateRowIndex(end);
            return _dictTable.Skip(start).Take(end).ToList();
        }
        public IEnumerable<IDictionary<string, TableCell>> GetRows(Func<IDictionary<string, TableCell>, bool> predicate)
        {
            return _dictTable.Where(predicate).ToList();
        }

        public void AddRow(IDictionary<string, TableCell> row)
        {
            ValidateRow(row);
            _dictTable.Add(row);
        }

        public IDictionary<string, TableCell> RemoveRow(int rowIndex)
        {
            var item = _dictTable[rowIndex];
            _dictTable.Remove(item);
            return item;
        }
        public IDictionary<string, TableCell> RemoveFirstRow()
        {
            var item = _dictTable[Count - 1];
            _dictTable.Remove(item);
            return item;
        }
        public IDictionary<string, TableCell> RemoveLastRow()
        {
            var item = _dictTable[Count - 1];
            _dictTable.Remove(item);
            return item;
        }
        public void RemoveRows(Func<IDictionary<string, TableCell>, bool> predicate)
        {
            foreach (IDictionary<string,TableCell> row in _dictTable.Where(predicate).ToList())
            {
                _dictTable.Remove(row);
            }
        }
        public void RemoveDefaultColumns()
        {
            var headersToRemove = Headers.Where(h => h.Text.Contains(_defaultHeaderName)).ToList();
            var rows = GetRows();

            foreach (var header in headersToRemove)
            {
                Headers.Remove(header);
                foreach (IDictionary<string,TableCell> row in rows)
                    row.Remove(header.Text);
            }
                
        }

        public bool IsEmpty()
        {
            return _dictTable.Count == 0;
        }

        public void Load(CancellationToken? token = null)
        {
            TableOptions options = new TableOptions();
            SetHeadersFromWebTable(_tableElement, token, options);
            SetBodyFromWebTable(_tableElement, token, options);
        }
        public void Load(TableOptions options, CancellationToken? token = null)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            SetParams();
            SetHeadersFromWebTable(_tableElement, token, options);
            SetBodyFromWebTable(_tableElement, token, options);
        }

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
                return new TableCell(element, $"{_defaultHeaderName}{_defaultTextIndex++}");

            return new TableCell(element, element.Text + additionalText);
        }

        private void ValidateTable(IWebElement table)
        {
            if (table.TagName.ToLower() != "table")
                throw new Exception($"The {table.TagName} is not a table");
        }
        private void ValidateRowIndex(int row)
        {
            if (row >= _dictTable.Count || row < 0)
                throw new ArgumentOutOfRangeException();
        }
        private void ValidateRow(IDictionary<string,TableCell> newRow)
        {
            int count = 0;
            foreach (string header in GetHeadersString())
            {
                if (newRow.ContainsKey(header))
                    count++;
            }

            if (count != Headers.Count)
                throw new InvalidOperationException("The row is not valid *check if the row has all the required table headers*");
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

            ReadOnlyCollection<IWebElement> trs = table.FindElements(By.XPath(".//tr[position() > 1 and ./ancestor::table[not(./descendant::thead)]]"));
            if (trs.Count == 0)
                trs = table.FindElements(By.XPath(".//tr[position() >= 1 and not(./parent::thead)]"));

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
            string trXpression = ".//tr[position() = 1]";
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
            tdOrTh = "./th | ./td";
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
