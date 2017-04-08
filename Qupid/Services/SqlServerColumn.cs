using System;
using System.Data;

namespace Qupid.Services
{
    public class SqlServerColumn
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
        public readonly string TableName;

        /// <summary>
        /// Column name.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// Column identification number.
        /// </summary>
        public readonly Int16 OrdinalPosition;

        /// <summary>
        /// Default value of the column
        /// </summary>
        public readonly string ColumnDefault;

        /// <summary>
        /// Nullability of the column.If this column allows NULL, this column returns YES.Otherwise, No is returned.
        /// </summary>
        public readonly bool IsNullable;

        /// <summary>
        /// System-supplied data type.
        /// </summary>
        public readonly string DataType;

        /// <summary>
        /// Maximum length, in characters, for binary data, character data, or text and image data.Otherwise, NULL is returned.
        /// </summary>
        public readonly Int32? CharacterMaximumLength;

        /// <summary>
        /// Maximum length, in bytes, for binary data, character data, or text and image data.Otherwise, NULL is returned.
        /// </summary>
        public readonly Int32? CharacterOctetLength;

        /// <summary>
        /// Precision of approximate numeric data, exact numeric data, integer data, or monetary data.Otherwise, NULL is returned.
        /// </summary>
        public readonly byte? NumericPrecision;

        /// <summary>
        /// //numeric_precision_radix Int16   
        /// </summary>
        public readonly Int16? NumericPrecisionRadix;

        /// <summary>
        /// Scale of approximate numeric data, exact numeric data, integer data, or monetary data.Otherwise, NULL is returned.
        /// </summary>
        public readonly Int32? NumericScale;

        /// <summary>
        /// Subtype code for datetime and SQL-92 interval data types.For other data types, NULL is returned.
        /// </summary>
        public readonly Int16? DateTimePrecision;

        /// <summary>
        /// Returns master, indicating the database in which the character set is located, if the column is character data or text data type. Otherwise, NULL is returned.
        /// </summary>
        public readonly string CharacterSetCatalog;

        /// <summary>
        /// Always returns NULL.
        /// </summary>
        public readonly string CharacterSetSchema;

        /// <summary>
        /// Returns the unique name for the character set if this column is character data or text data type. Otherwise, NULL is returned.
        /// </summary>
        public readonly string CharacterSetName;

        /// <summary>
        /// Returns master, indicating the database in which the collation is defined, if the column is character data or text data type. Otherwise, this column is NULL.
        /// </summary>
        public readonly string CollationCatalog;

        public string ConstraintName { get; set; }

        public bool IsPrimaryKey { get; set; }

        public SqlServerTable Table { get; set; }


        public SqlServerColumn(DataRow dataRow)
        {
            Catalog = dataRow["table_catalog"] as string;

            Schema = dataRow["table_schema"] as string;

            TableName = dataRow["table_name"] as string;

            Name = dataRow["column_name"] as string;

            OrdinalPosition = Convert.ToInt16(dataRow["ordinal_position"]);

            ColumnDefault = dataRow["column_default"] as string;

            if (dataRow["is_nullable"] as string == "YES")
            {
                IsNullable = true;
            }
            else if (dataRow["is_nullable"] as string == "NO")
            {
                IsNullable = false;
            }

            DataType = dataRow["data_type"] as string;

            if (dataRow["character_maximum_length"] is DBNull)
            {
                CharacterMaximumLength = null;
            }
            else
            {
                CharacterMaximumLength = Convert.ToInt32(dataRow["character_maximum_length"]);
            }

            if (dataRow["character_octet_length"] is DBNull)
            {
                CharacterOctetLength = null;
            }
            else
            {
                CharacterOctetLength = Convert.ToInt32(dataRow["character_octet_length"]);
            }

            if (dataRow["numeric_precision"] is DBNull)
            {
                NumericPrecision = null;
            }
            else
            {
                NumericPrecision = dataRow["numeric_precision"] as byte?;
            }

            if (dataRow["numeric_precision_radix"] is DBNull)
            {
                NumericPrecisionRadix = null;
            }
            else
            {
                NumericPrecisionRadix = Convert.ToInt16(dataRow["numeric_precision_radix"]);
            }

            if (dataRow["numeric_scale"] is DBNull)
            {
                NumericScale = null;
            }
            else
            {
                NumericScale = Convert.ToInt32(dataRow["numeric_scale"]);
            }

            if (dataRow["datetime_precision"] is DBNull)
            {
                DateTimePrecision = null;
            }
            else
            {
                DateTimePrecision = Convert.ToInt16(dataRow["datetime_precision"]);
            }

            CharacterSetCatalog = dataRow["character_set_catalog"] as string;

            CharacterSetSchema = dataRow["character_set_schema"] as string;

            CharacterSetName = dataRow["character_set_name"] as string;

            CollationCatalog = dataRow["collation_catalog"] as string;
        }
    }
}
