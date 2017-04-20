using DbExtensions;
using Qupid.Services;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Qupid
{
    public class SqlServerSchema
    {
        public string ConnectionString { get; private set; }

        public List<SqlServerTable> Tables { get; private set; } = new List<SqlServerTable>();

        public List<SqlServerColumn> Columns { get; private set; } = new List<SqlServerColumn>();

        public List<SqlServerPrimaryKeyColumn> PrimaryKeyColumns { get; private set; } = new List<SqlServerPrimaryKeyColumn>();

        public SqlServerSchema(string connectionString)
        {
            ConnectionString = connectionString;

            LoadSchema();
        }

        private void LoadSchema()
        {
            SqlServerService sqlServerService = new SqlServerService(ConnectionString, null);

            string primaryKeyQuery;

            SqlBuilder sqlBuilder = new SqlBuilder();

            sqlBuilder.SELECT("c.TABLE_CATALOG,c.TABLE_SCHEMA,c.TABLE_NAME,c.CONSTRAINT_NAME,c.CONSTRAINT_TYPE,u.COLUMN_NAME");
            sqlBuilder.FROM("INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS c");
            sqlBuilder.INNER_JOIN("INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS u ON c.CONSTRAINT_NAME = u.CONSTRAINT_NAME");
            sqlBuilder.WHERE("c.CONSTRAINT_TYPE = 'PRIMARY KEY'");

            primaryKeyQuery = sqlBuilder.ToString();

            DataTable primaryKeyColumnSchema = sqlServerService.GenericExecuteResultQuery(primaryKeyQuery);

            foreach (DataRow primaryKeyColumnSchemaRow in primaryKeyColumnSchema.Rows)
            {
                SqlServerPrimaryKeyColumn primaryKeyColumn = new SqlServerPrimaryKeyColumn(primaryKeyColumnSchemaRow);

                PrimaryKeyColumns.Add(primaryKeyColumn);
            }

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();
                
                DataTable columnSchema = sqlConnection.GetSchema("Columns");

                foreach (DataRow columnSchemaRow in columnSchema.Rows)
                {
                    SqlServerColumn column = new SqlServerColumn(columnSchemaRow);

                    SqlServerPrimaryKeyColumn primaryKeyColumn = PrimaryKeyColumns.Find(pkc => pkc.Name == column.Name && pkc.TableName == column.TableName && pkc.Schema == column.Schema && pkc.Catalog == column.Catalog);

                    if (primaryKeyColumn != null)
                    {
                        column.ConstraintName = primaryKeyColumn.ConstraintName;
                        column.IsPrimaryKey = true;
                    }

                    Columns.Add(column);
                }

                DataTable tableSchema = sqlConnection.GetSchema("Tables");
                                
                foreach (DataRow tableSchemaRow in tableSchema.Rows)
                {
                    SqlServerTable table = new SqlServerTable(tableSchemaRow);
                    Tables.Add(table);

                    List<SqlServerColumn> columns = Columns.FindAll(c => c.Schema == table.Schema && c.TableName == table.Name);

                    foreach (SqlServerColumn column in columns)
                    {
                        column.Table = table;
                        table.Columns.Add(column);                        
                    }
                }
                
                sqlConnection.Close();
            }
        }
    }
}
