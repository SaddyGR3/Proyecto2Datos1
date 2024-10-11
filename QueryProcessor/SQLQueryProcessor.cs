using Entities;
using QueryProcessor.Exceptions;
using QueryProcessor.Operations;
using StoreDataManager;

namespace QueryProcessor
{
    public class SQLQueryProcessor
    {
        // Variable para almacenar la base de datos actual
        private static string currentDatabase = null;

        public static (OperationStatus, List<string>) Execute(string sentence)
        {
            // SET DATABASE
            if (sentence.StartsWith("SET DATABASE", StringComparison.OrdinalIgnoreCase))
            {
                var dbName = sentence.Split("DATABASE")[1]?.Trim(' ', ';');
                if (string.IsNullOrEmpty(dbName))
                {
                    Console.WriteLine("Error: No se encontró el nombre de la base de datos.");
                    return (OperationStatus.Error, null);
                }

                // Actualiza la base de datos actual
                currentDatabase = dbName;
                Console.WriteLine($"Base de datos actualizada: {currentDatabase}");
                return (OperationStatus.Success, null);
            }

            // CREATE DATABASE
            if (sentence.StartsWith("CREATE DATABASE", StringComparison.OrdinalIgnoreCase))
            {
                var dbName = sentence.Split("DATABASE")[1]?.Trim(' ', ';');
                if (string.IsNullOrEmpty(dbName))
                {
                    Console.WriteLine("Error: No se encontró el nombre de la base de datos.");
                    return (OperationStatus.Error, null);
                }

                // Llamamos a la operación CreateDatabase para crear la base de datos
                var status = new CreateDatabase().Execute(dbName); // Aquí solo pasamos el nombre de la base de datos
                return (status, null);
            }
            // CREATE TABLE
            if (sentence.StartsWith("CREATE TABLE", StringComparison.OrdinalIgnoreCase))
            {
                var tableName = sentence.Split("TABLE")[1]?.Split('(')[0]?.Trim();
                if (string.IsNullOrEmpty(tableName))
                {
                    Console.WriteLine("Error: No se encontró el nombre de la tabla.");
                    return (OperationStatus.Error, null);
                }

                Console.WriteLine($"Nombre de la tabla extraída: {tableName}");

                // Verifica que hay una base de datos seleccionada
                if (string.IsNullOrEmpty(currentDatabase))
                {
                    Console.WriteLine("Error: No se ha seleccionado ninguna base de datos.");
                    return (OperationStatus.Error, null);
                }

                Console.WriteLine($"Base de datos actual para CREATE TABLE: {currentDatabase}");
                Console.WriteLine($"Query recibido para CREATE TABLE: {sentence}");

                // Llama a la operación CreateTable con la base de datos y la tabla
                var status = new CreateTable().Execute(currentDatabase, tableName, sentence);
                Console.WriteLine($"Resultado de CREATE TABLE: {status}");
                return (status, null);
            }

            // INSERT
            if (sentence.StartsWith("INSERT", StringComparison.OrdinalIgnoreCase))
            {
                var tableName = sentence.Split("INTO")[1].Split("VALUES")[0]?.Trim();
                if (string.IsNullOrEmpty(tableName))
                {
                    Console.WriteLine("Error: No se encontró el nombre de la tabla.");
                    return (OperationStatus.Error, null);
                }

                // Verifica que hay una base de datos seleccionada
                if (string.IsNullOrEmpty(currentDatabase))
                {
                    Console.WriteLine("Error: No se ha seleccionado ninguna base de datos.");
                    return (OperationStatus.Error, null);
                }

                // Llama a la operación Insert con la base de datos, la tabla y la sentencia
                var status = new Insert().Execute(currentDatabase, tableName, sentence);
                return (status, null);
            }

            // DELETE
            if (sentence.StartsWith("DELETE", StringComparison.OrdinalIgnoreCase))
            {
                var tableName = sentence.Split("FROM")[1].Split("WHERE")[0]?.Trim();
                if (string.IsNullOrEmpty(tableName))
                {
                    Console.WriteLine("Error: No se encontró el nombre de la tabla.");
                    return (OperationStatus.Error, null);
                }

                // Verifica que hay una base de datos seleccionada
                if (string.IsNullOrEmpty(currentDatabase))
                {
                    Console.WriteLine("Error: No se ha seleccionado ninguna base de datos.");
                    return (OperationStatus.Error, null);
                }

                // Llama a la operación Delete con la base de datos y la tabla
                var status = new Delete().Execute(currentDatabase, tableName, sentence);
                return (status, null);
            }

            // SELECT
            if (sentence.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
            {
                var tableName = sentence.Split("FROM")[1]?.Trim();
                if (string.IsNullOrEmpty(tableName))
                {
                    Console.WriteLine("Error: No se encontró el nombre de la tabla.");
                    return (OperationStatus.Error, null);
                }

                // Verifica que hay una base de datos seleccionada
                if (string.IsNullOrEmpty(currentDatabase))
                {
                    Console.WriteLine("Error: No se ha seleccionado ninguna base de datos.");
                    return (OperationStatus.Error, null);
                }

                // Llama a la operación Select con la base de datos y la tabla
                return new Select().Execute(currentDatabase, tableName, sentence);
            }
            // DROP TABLE
            if (sentence.StartsWith("DROP TABLE", StringComparison.OrdinalIgnoreCase))
            {
                var tableName = sentence.Split("TABLE")[1]?.Trim(' ', ';');
                if (string.IsNullOrEmpty(tableName))
                {
                    Console.WriteLine("Error: No se encontró el nombre de la tabla.");
                    return (OperationStatus.Error, null);
                }

                // Verifica que hay una base de datos seleccionada
                if (string.IsNullOrEmpty(currentDatabase))
                {
                    Console.WriteLine("Error: No se ha seleccionado ninguna base de datos.");
                    return (OperationStatus.Error, null);
                }

                // Llama a la operación DropTable con la base de datos y la tabla
                //var status = new DropTable().Execute(currentDatabase, tableName);
                //return (status, null);
            }
            // UPDATE
            if (sentence.StartsWith("UPDATE", StringComparison.OrdinalIgnoreCase))
            {
                var tableName = sentence.Split("UPDATE")[1]?.Split("SET")[0]?.Trim();
                if (string.IsNullOrEmpty(tableName))
                {
                    Console.WriteLine("Error: No se encontró el nombre de la tabla.");
                    return (OperationStatus.Error, null);
                }

                // Verifica que hay una base de datos seleccionada
                if (string.IsNullOrEmpty(currentDatabase))
                {
                    Console.WriteLine("Error: No se ha seleccionado ninguna base de datos.");
                    return (OperationStatus.Error, null);
                }

                // Llama a la operación Update con la base de datos, la tabla y la sentencia
                var status = new Update().Execute(currentDatabase, tableName, sentence);
                return (status, null);
            }
            // Si no se reconoce la sentencia
            Console.WriteLine("Error: Sentencia SQL no reconocida.");
            throw new UnknownSQLSentenceException();  // Excepción para sentencias desconocidas
        }
    }
}
