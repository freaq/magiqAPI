using System.Data;

namespace Qupid.Services
{
    public class SqlServerPrimaryKeyColumn
    {
        public readonly string Catalog;

        public readonly string Schema;

        public readonly string TableName;

        public readonly string ConstraintName;

        public readonly string ConstraintType;

        public readonly string Name;


        public SqlServerPrimaryKeyColumn(DataRow dataRow)
        {
            Catalog = dataRow["table_catalog"] as string;

            Schema = dataRow["table_schema"] as string;

            TableName = dataRow["table_name"] as string;

            ConstraintName = dataRow["constraint_name"] as string;

            ConstraintType = dataRow["constraint_type"] as string;

            Name = dataRow["column_name"] as string;
        }

    }
}
