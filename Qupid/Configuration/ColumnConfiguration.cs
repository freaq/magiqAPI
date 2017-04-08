namespace Qupid.Configuration
{
    public class ColumnConfiguration
    {
        public string ColumnName { get; set; }

        public string PropertyName { get; set; }

        public bool IsPrimaryKey { get; set; }

        public ColumnConfiguration()
        {

        }
    }
}