namespace Entities
{
    public enum OperationStatus
    {
        Success,
        Error,
        Warning,
        Status,
        Info,
        Fatal,
        TableAlreadyExists,
        DatabaseAlreadyExists,
        DatabaseNotFound,
        TableNotFound
    }
    public enum DataType
    {
        Integer,
        Varchar,
        Datetime,
        String
    }
    public class ColumnDefinition
    {
        public string ColumnName { get; set; }
        public DataType DataType { get; set; }
        public int? MaxLength { get; set; }
    }
}
