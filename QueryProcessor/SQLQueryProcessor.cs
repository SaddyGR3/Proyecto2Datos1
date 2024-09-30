using Entities;
using QueryProcessor.Exceptions;
using QueryProcessor.Operations;
using QueryProcessor.Parser;
using StoreDataManager;


namespace QueryProcessor
{
    public class SQLQueryProcessor
    {
        public static OperationStatus Execute(string sentence)
        {
            // Usamos el parser para descomponer la sentencia SQL
            ParsedQuery parsedQuery = SQLParser.Parse(sentence);

            // Dependiendo del tipo de comando, llamamos a la operación correspondiente
            switch (parsedQuery.CommandType)
            {
                case CommandType.CreateTable:
                    return new CreateTable().Execute(parsedQuery);

                case CommandType.Insert:
                    return new Insert().Execute(parsedQuery);

                case CommandType.Select:
                    return new Select().Execute(parsedQuery);

                default:
                    throw new UnknownSQLSentenceException(sentence);
            }
        }
    }
}
