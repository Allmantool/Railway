namespace Railway.DeliveryCargo.Data.EF.Domain
{
    using System.Data;

    public class DatabaseParameter
    {
        /// <summary>
        /// Gets or sets the full parameter name as found in the query with which the
        /// parameter is associated.
        /// </summary>
        public string ParameterName { get; set; }

        /// <summary>
        /// Gets or sets the value of the parameter.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Gets or sets the type of the parameter. This value is optional.
        /// </summary>
        public DbType? Type { get; set; }

        /// <summary>
        /// Gets or sets the size of the parameter. This value should be set to the size
        /// of the underlying database column, not the size of the parameter value. This value
        /// is optional but recommended for string fields as setting it will ensure that the
        /// parameter size provided to the database is consistent between executions, which is
        /// required for the execution plan cache to be utilized.
        /// </summary>
        public int? DataSize { get; set; }

        /// <summary>
        /// Gets or sets parameter within a query relative to the System.Data.DataSet.
        /// </summary>
        public ParameterDirection Direction { get; set; }

        public int Size { get; set; }
    }
}
