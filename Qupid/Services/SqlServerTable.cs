using System.Collections.Generic;
using System.Data;

namespace Qupid.Services
{
    public class SqlServerTable
    {
        /// <summary>
        /// Catalog of the table.
        /// </summary>
        public readonly string Catalog;

        /// <summary>
        /// Schema that contains the table.
        /// </summary>
        public readonly string Schema;

        /// <summary>
        /// Table name.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// Type of table. Can be VIEW or BASE TABLE.
        /// </summary>
        public readonly SqlServerTableType Type;

        public List<SqlServerColumn> Columns { get; private set; } = new List<SqlServerColumn>();

        public readonly string FullTableName;

        public SqlServerTable(DataRow dataRow)
        {
            Catalog = dataRow["table_catalog"] as string;

            Schema = dataRow["table_schema"] as string;

            Name = dataRow["table_name"] as string;

            if (dataRow.Table.Columns.Contains("table_type"))
            {
                if (dataRow["table_type"] as string == "VIEW")
                {
                    Type = SqlServerTableType.View;
                }
                else if (dataRow["table_type"] as string == "BASE TABLE")
                {
                    Type = SqlServerTableType.BaseTable;
                }
                else
                {
                    Type = SqlServerTableType.Unknown;
                }
            }
            else
            {
                Type = SqlServerTableType.Unknown;
            }

            FullTableName = Schema + "." + Name;
        }
    }

    public enum SqlServerTableType
    {
        Unknown,
        View,
        BaseTable
    }
}
