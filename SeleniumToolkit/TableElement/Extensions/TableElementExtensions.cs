using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumToolkit.TableElements.Extensions
{
    public static class TableElementExtensions
    {
        /// <summary>
        /// Creates a new datatable object with the current table element
        /// </summary>
        /// <returns>A datatable that represents the table element (by default all values of datatable are strings)</returns>
        public static DataTable CreateDataTable(this TableElement tableElement)
        {
            DataTable tb = new();

            string[] headers = tableElement.GetHeaders().ToArray();
            foreach (string column in headers)
                tb.Columns.Add(column);

            foreach (var row in tableElement.GetRows())
            {
                DataRow newRow = tb.NewRow();
                foreach (string column in headers)
                    newRow[column] = row[column].Text;

                tb.Rows.Add(newRow);
            }
            return tb;
        }

        /// <summary>
        /// Creates a new datatable object with the current table element
        /// </summary>
        /// <param name="cellCallback">A callback to get the desired value for each cell in the table element</param>
        /// <returns>A datatable that represents the table element</returns>
        public static DataTable CreateDataTable(this TableElement tableElement, Func<TableCell, object> cellCallback)
        {
            DataTable tb = new();

            string[] headers = tableElement.GetHeaders().ToArray();
            foreach (string column in headers)
                tb.Columns.Add(column);

            foreach (var row in tableElement.GetRows())
            {
                DataRow newRow = tb.NewRow();
                foreach (string column in headers)
                    newRow[column] = cellCallback(row[column]);

                tb.Rows.Add(newRow);
            }
            return tb;
        }
    }
}
