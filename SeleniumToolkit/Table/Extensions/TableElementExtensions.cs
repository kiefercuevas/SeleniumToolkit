using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumToolkit.Table.Extensions
{
    public static class TableElementExtensions
    {
        /// <summary>
        /// Creates a new datatable object with the current table element
        /// </summary>
        /// <returns>A datatable that represents the table element (by default all values of datatable are strings)</returns>
        public static DataTable CreateDataTable(this ITableElement tableElement)
        {
            DataTable tb = new DataTable();

            foreach (TableCell column in tableElement.Headers)
                tb.Columns.Add(column.Text);

            foreach (var row in tableElement.GetRows())
            {
                DataRow newRow = tb.NewRow();
                foreach (TableCell column in tableElement.Headers)
                    newRow[column.Text] = row[column.Text].Text;

                tb.Rows.Add(newRow);
            }
            return tb;
        }

        /// <summary>
        /// Creates a new datatable object with the current table element
        /// </summary>
        /// <param name="cellCallback">A callback to set the desired value for each cell in the table element</param>
        /// <returns>A datatable that represents the table element</returns>
        public static DataTable CreateDataTable(this ITableElement tableElement, Func<TableCell, object> cellCallback)
        {
            DataTable tb = new DataTable();

            foreach (TableCell column in tableElement.Headers)
                tb.Columns.Add(column.Text);

            foreach (var row in tableElement.GetRows())
            {
                DataRow newRow = tb.NewRow();
                foreach (TableCell column in tableElement.Headers)
                    newRow[column.Text] = cellCallback(row[column.Text]);

                tb.Rows.Add(newRow);
            }
            return tb;
        }

        /// <summary>
        /// Creates a new datatable object with the current table element
        /// </summary>
        /// <param name="rowCallback">A callback to get the desired rows base on a condition</param>
        public static DataTable CreateDataTable(this ITableElement tableElement, Func<IDictionary<string, TableCell>, bool> rowCallback)
        {
            DataTable tb = new DataTable();

            foreach (TableCell column in tableElement.Headers)
                tb.Columns.Add(column.Text);

            foreach (var row in tableElement.GetRows(rowCallback).ToList())
            {
                DataRow newRow = tb.NewRow();
                foreach (TableCell column in tableElement.Headers)
                    newRow[column.Text] = row[column.Text].Text;

                tb.Rows.Add(newRow);
            }
            return tb;
        }

        /// <summary>
        /// Creates a new datatable object with the current table element
        /// </summary>
        /// <param name="rowCallback">A callback to get the desired rows</param>
        public static DataTable CreateDataTable(this ITableElement tableElement, Func<IEnumerable<IDictionary<string, TableCell>>, IEnumerable<IDictionary<string, TableCell>>> rowCallback)
        {
            DataTable tb = new DataTable();

            foreach (TableCell column in tableElement.Headers)
                tb.Columns.Add(column.Text);

            foreach (var row in rowCallback(tableElement.GetRows()))
            {
                DataRow newRow = tb.NewRow();
                foreach (TableCell column in tableElement.Headers)
                    newRow[column.Text] = row[column.Text].Text;

                tb.Rows.Add(newRow);
            }
            return tb;
        }
    }
}
