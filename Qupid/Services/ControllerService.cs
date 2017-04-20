using DbExtensions;
using Newtonsoft.Json;
using Qupid.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qupid.Services
{
    public class ControllerService
    {
        //public static DataTable ApplyColumnMapping(DataTable)
        //{

        //}

        public static string GetDefaultGetAllQuery(RouteConfiguration route)
        {
            string sqlQuery;

            SqlBuilder sqlBuilder = new SqlBuilder();

            sqlBuilder = AddSELECTQuery(sqlBuilder, route);

            sqlBuilder = AddFROMQuery(sqlBuilder, route);

            sqlQuery = sqlBuilder.ToString();

            return sqlQuery;
        }

        public static string GetDefaultGetByIntQuery(RouteConfiguration route, int id)
        {
            string sqlQuery;

            SqlBuilder sqlBuilder = new SqlBuilder();

            sqlBuilder = AddSELECTQuery(sqlBuilder, route);

            sqlBuilder = AddFROMQuery(sqlBuilder, route);

            sqlBuilder.WHERE(route.PrimaryKeyColumn + " = " + id);

            sqlQuery = sqlBuilder.ToString();

            return sqlQuery;
        }

        public static string GetDefaultGetByStringQuery(RouteConfiguration route, string id)
        {
            string sqlQuery;

            SqlBuilder sqlBuilder = new SqlBuilder();

            sqlBuilder = AddSELECTQuery(sqlBuilder, route);

            sqlBuilder = AddFROMQuery(sqlBuilder, route);

            sqlBuilder.WHERE(route.PrimaryKeyColumn + " = '" + id + "'");

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

                if (jsonProperty.Value is string)
                {
                    columnValues.Add("'" + jsonProperty.Value + "'");
                }
                else if (jsonProperty.Value is int)
                {
                    columnValues.Add(jsonProperty.Value);
                }
            }

            string columnParameters = String.Join(",", columnNames);
            object[] valueParameters = columnValues.ToArray();

            string into = GetSchemaTableName(route);

            sqlBuilder.INSERT_INTO(into + "(" + columnParameters + ")");

            sqlBuilder.VALUES(valueParameters);

            sqlQuery = sqlBuilder.ToString();

            sqlQuery = string.Format(sqlQuery, valueParameters);

            return sqlQuery;
        }

        public static string GetDefaultPutQuery(RouteConfiguration route, int id, string json)
        {
            string sqlQuery;

            SqlBuilder sqlBuilder = new SqlBuilder();

            Dictionary<string, object> jsonDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

            List<string> columnNames = new List<string>();
            List<object> columnValues = new List<object>();
            List<string> setParameters = new List<string>();
            int index = 0;
            foreach (KeyValuePair<string, object> jsonProperty in jsonDictionary)
            {
                columnNames.Add(jsonProperty.Key);

                setParameters.Add(jsonProperty.Key + " = {" + index + "}");

                if (jsonProperty.Value is string)
                {
                    columnValues.Add("'" + jsonProperty.Value + "'");
                }
                else if (jsonProperty.Value is int)
                {
                    columnValues.Add(jsonProperty.Value);
                }

                index++;
            }

            string setParameter = String.Join(",", setParameters);
            object[] valueParameters = columnValues.ToArray();
            
            string updateFrom = GetSchemaTableName(route);

            sqlBuilder.UPDATE(updateFrom);

            sqlBuilder.SET(setParameter);

            sqlBuilder.WHERE(route.PrimaryKeyColumn + " = " + id);

            sqlQuery = sqlBuilder.ToString();

            sqlQuery = string.Format(sqlQuery, valueParameters);

            return sqlQuery;
        }

        public static string GetDefaultDeleteQuery(RouteConfiguration route, int id)
        {
            string sqlQuery;

            SqlBuilder sqlBuilder = new SqlBuilder();

            string deleteFrom = GetSchemaTableName(route);

            sqlBuilder.DELETE_FROM(deleteFrom);

            sqlBuilder.WHERE(route.PrimaryKeyColumn + " = " + id);

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
            string from = GetSchemaTableName(route);
            return sqlBuilder.FROM(from);
        }

        private static string GetSchemaTableName(RouteConfiguration route)
        {
            string schemaTableName;

            // if a table schema is configured
            // then use 'schema.table'
            // else use 'table'
            if (!String.IsNullOrEmpty(route.Schema))
            {
                schemaTableName = route.Schema + "." + route.Table;
            }
            else
            {
                schemaTableName = route.Table;
            }

            return schemaTableName;
        }
    }
}
