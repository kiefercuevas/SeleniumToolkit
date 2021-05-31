using SeleniumToolkit.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumToolkit.Table.Extensions
{
    public static class TableElementJoinExtentions
    {
        /// <summary>
        /// Join the current table element with a new one
        /// </summary>
        /// <param name="newTable">The table to append</param>
        public static void Join(this ITableElement currentTable, ITableElement newTable)
        {
            ValidateHeaders(currentTable, newTable);
            foreach (var row in newTable.GetRows())
            {
                currentTable.AddRow(row);
            }
        }

        private static void ValidateHeaders(ITableElement currentTable,  ITableElement newTable)
        {
            int count = 0;
            var headers = currentTable.GetHeadersDict();
            foreach (string newTableHeader in newTable.GetHeadersString())
            {
                if (headers.ContainsKey(newTableHeader))
                    count++;
            }

            if (count != currentTable.Headers.Count)
                throw new InvalidOperationException("The table have different headers");
        }
    }
}
