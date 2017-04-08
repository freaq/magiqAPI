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

        public SqlServerSchema(string connectionString)
        {
            ConnectionString = connectionString;

            LoadSchema();
        }

        private void LoadSchema()
        {
            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();

                DataTable columnSchema = sqlConnection.GetSchema("Columns");

                foreach (DataRow columnSchemaRow in columnSchema.Rows)
                {
                    SqlServerColumn column = new SqlServerColumn(columnSchemaRow);
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
