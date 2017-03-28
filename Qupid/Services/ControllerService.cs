using DbExtensions;
using Newtonsoft.Json;
using Qupid.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Qupid.Services
{
    public class ControllerService
    {
        public static string GetDefaultGetAllQuery(RouteConfiguration route)
        {
            string sqlQuery;

            SqlBuilder sqlBuilder = new SqlBuilder();

            sqlBuilder = AddSELECTQuery(sqlBuilder, route);

            sqlBuilder = AddFROMQuery(sqlBuilder, route);

            sqlQuery = sqlBuilder.ToString();

            return sqlQuery;
        }

        public static string GetDefaultGetQuery(RouteConfiguration route, int id)
        {
            string sqlQuery;

            SqlBuilder sqlBuilder = new SqlBuilder();

            sqlBuilder = AddSELECTQuery(sqlBuilder, route);

            sqlBuilder = AddFROMQuery(sqlBuilder, route);

            sqlBuilder.WHERE(route.PrimaryKeyColumn + " = " + id);

            sqlQuery = sqlBuilder.ToString();

            return sqlQuery;
        }

        public static string GetDefaultPostQuery(RouteConfiguration route, string json)
        {
            string sqlQuery;

            SqlBuilder sqlBuilder = new SqlBuilder();

            Dictionary<string, object> jsonDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

            List<string> columnNames = new List<string>();
            List<object> columnValues = new List<object>();
            foreach (KeyValuePair<string, object> jsonProperty in jsonDictionary)
            {
                columnNames.Add(jsonProperty.Key);
                columnValues.Add(jsonProperty.Value);
            }


            //         var insert = SQL
            //.INSERT_INTO("Products(ProductName, UnitPrice, CategoryID)")
            //.VALUES("Chai", 15.56, 5);



            string columnParameters = String.Join(",", columnNames);
            object[] valueParameters = columnValues.ToArray();

            sqlBuilder.INSERT_INTO(route.Table + "(" + columnParameters + ")").VALUES("bla", 123);

            sqlBuilder.VALUES(valueParameters);

            sqlQuery = sqlBuilder.ToString();

            return sqlQuery;
        }

        private static SqlBuilder AddSELECTQuery(SqlBuilder sqlBuilder, RouteConfiguration route)
        {
            // if route columns are configured
            // then only select those columns from the database
            // else select all columns
            if (route.Columns.Any())
            {
                foreach (ColumnConfiguration column in route.Columns)
                {
                    sqlBuilder.SELECT(column.ColumnName);
                }
            }
            else
            {
                sqlBuilder.SELECT("*");
            }

            return sqlBuilder;
        }

        private static SqlBuilder AddFROMQuery(SqlBuilder sqlBuilder, RouteConfiguration route)
        {
            // if a table schema is configured
            // then select from 'schema.table'
            // else select from 'table'
            if (!String.IsNullOrEmpty(route.Schema))
            {
                sqlBuilder.FROM(route.Schema + "." + route.Table);
            }
            else
            {
                sqlBuilder.FROM(route.Table);
            }

            return sqlBuilder;
        }

    }
}
