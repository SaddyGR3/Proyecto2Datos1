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
            //create table
            if (sentence.StartsWith("CREATE TABLE", StringComparison.OrdinalIgnoreCase))
            {
                var status = new CreateTable().Execute(sentence);  // Ejecuta la sentencia CREATE TABLE
                return (status, null);  // Solo devuelve el estado para CREATE TABLE
            }
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
