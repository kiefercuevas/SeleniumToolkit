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
        /// <returns>A datatable that represents the table element</returns>
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
    }
}
