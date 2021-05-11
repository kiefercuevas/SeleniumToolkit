namespace SeleniumToolkit.TableElements
{
    /// <summary>
    /// Provide some options to create the table
    /// </summary>
    public class TableOptions
    {
        /// <summary>
        /// Is this options is set to true the return the table only with headers
        /// </summary>
        public bool GetEmptyTable { get; set; } = false;
        /// <summary>
        /// The number of the row to start,*if StartRow is less than 0 then start from 0* default: 0
        /// </summary>
        public int StartRow { get; set; } = 0;
        /// <summary>
        /// The number of rows to take, *if RowAmount is less than 0 then take all the rows*, default: -1
        /// </summary>
        public int RowAmount { get; set; } = -1;
    }
}
