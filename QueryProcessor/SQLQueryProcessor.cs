using Entities;
using QueryProcessor.Exceptions;
using QueryProcessor.Operations;
using StoreDataManager;

namespace QueryProcessor
{
    public class SQLQueryProcessor
    {
        public static (OperationStatus, List<string>) Execute(string sentence)
        {
            // INSERT
            if (sentence.StartsWith("INSERT", StringComparison.OrdinalIgnoreCase))
            {
                var status = new Insert().Execute(sentence);
                return (status, null);  // Solo devuelve el estado para INSERT
            }

            // DELETE
            if (sentence.StartsWith("DELETE", StringComparison.OrdinalIgnoreCase))
            {
                var status = new Delete().Execute(sentence);
                return (status, null);  // Solo devuelve el estado para DELETE
            }

            // SELECT
            if (sentence.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
            {
                return new Select().Execute(sentence);  // Devuelve el estado y los resultados para SELECT
            }

            // Si no se reconoce la sentencia
            throw new UnknownSQLSentenceException();
        }
    }
}
