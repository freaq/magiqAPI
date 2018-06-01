using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
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

        private const string SCHEMA_SYMBOL = "{$schema}";
        private const string TABLE_SYMBOL = "{$table}";
        private const string SCHEMA_TABLE_SYMBOL = "{$schemaTable}";
        private const string COLUMN_NAMES_SYMBOL = "{$columnNames}";
        private const string PRIMARY_KEY_COLUMN_SYMBOL = "{$primaryKeyColumn}";
        private const string ID_SYMBOL = "{$id}";
        private const string BODY_COLUMN_NAMES_SYMBOL = "{$bodyColumnNames}";
        private const string BODY_COLUMN_VALUES_SYMBOL = "{$bodyColumnValues}";
        private const string BODY_COLUMN_NAMES_AND_VALUES_SYMBOL = "{$bodyColumnNamesAndValues}";

        public static string GetStringReplacedQuery(HttpRequest request, RouteConfiguration route, ActionConfiguration action, string json = null)
        {
            if (String.IsNullOrEmpty(action.Query))
            {
                throw new Exception("The action query is required.");
            }

            string sqlQuery = action.Query;

            sqlQuery = sqlQuery.Replace(SCHEMA_SYMBOL, route.Schema);
            sqlQuery = sqlQuery.Replace(TABLE_SYMBOL, route.Table);
            sqlQuery = sqlQuery.Replace(SCHEMA_TABLE_SYMBOL, GetSchemaTableName(route));

            // extract OData query information
            List<string> odataSelectColumns = new List<string>();
            //string odataTopValue = null;
            foreach (KeyValuePair<String, StringValues> query in request.Query)
            {                
                if (query.Key.ToLower() == "$select")
                {
                    odataSelectColumns = query.Value.ToString().Split(',').ToList();
                }
                //else if (query.Key.ToLower() == "$top")
                //{
                //    odataTopValue = query.Value.ToString();
                //}
            }


            List<string> selectString = new List<string>();
            if (route.Columns.Any())
            {
                if (odataSelectColumns.Count > 0)
                {
                    foreach (string odataSelectColumn in odataSelectColumns)
                    {
                        if (route.Columns.Find(cc => cc.ColumnName == odataSelectColumn) != null)
                        {
                            selectString.Add(odataSelectColumn);
                        }
                        else
                        {
                            throw new Exception("The OData $SELECT column '" + odataSelectColumn + "' is not a configured property for route '" + route.Name + "'.");
                        }
                    }
                }
                else
                {
                    foreach (ColumnConfiguration column in route.Columns)
                    {
                        selectString.Add(column.ColumnName);
                    }
                }                    
            }
            else
            {
                if (odataSelectColumns.Count > 0)
                {
                    selectString = odataSelectColumns;
                }
                else
                {
                    selectString.Add("*");
                }
            }
            
            sqlQuery = sqlQuery.Replace(COLUMN_NAMES_SYMBOL, String.Join(",", selectString));

            //if (odataTopValue != null)
            //{
            //    int indexOfFirstSelect = sqlQuery.ToLower().IndexOf("select");
            //    sqlQuery = sqlQuery.Insert(indexOfFirstSelect + 6, " TOP " + odataTopValue);
            //}

            sqlQuery = sqlQuery.Replace(PRIMARY_KEY_COLUMN_SYMBOL, route.PrimaryKeyColumn);

            if (!String.IsNullOrEmpty(json))
            {
                sqlQuery = ReplaceBodyColumnNamesAndValues(sqlQuery, json);
            }

            return sqlQuery;
        }

        public static string GetStringReplacedQuery(HttpRequest request, RouteConfiguration route, ActionConfiguration action, object id, string json = null)
        {
            string sqlQuery = GetStringReplacedQuery(request, route, action, json);

            sqlQuery = sqlQuery.Replace(ID_SYMBOL, id.ToString());

            return sqlQuery;
        }

        private static string ReplaceBodyColumnNamesAndValues(string sql, string json)
        {
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

            string columnParameters = String.Join(",", columnNames);
            string valueParameterString = String.Join(",", columnValues);
            string setParameter = String.Join(",", setParameters);
            object[] valueParameterArray = columnValues.ToArray();
            setParameter = string.Format(setParameter, valueParameterArray);

            string replacedSql = sql.Replace(BODY_COLUMN_NAMES_SYMBOL, columnParameters);
            replacedSql = replacedSql.Replace(BODY_COLUMN_VALUES_SYMBOL, valueParameterString);
            replacedSql = replacedSql.Replace(BODY_COLUMN_NAMES_AND_VALUES_SYMBOL, setParameter);

            return replacedSql;
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
