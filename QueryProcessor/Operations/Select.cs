using Entities;
using StoreDataManager;

namespace QueryProcessor.Operations
{
    internal class Select
    {
        public (OperationStatus, List<string>) Execute(string databaseName, string tableName, string sentence)
        {
            // Asegúrate de que la sentencia comienza con SELECT
            if (!sentence.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Error: La sentencia no comienza con SELECT.");
                return (OperationStatus.Error, null);
            }

            // Llamar al Store Data Manager para seleccionar desde la tabla especificada en la base de datos
            var (status, records) = Store.GetInstance().SelectFromTable(databaseName, tableName);

            if (status == OperationStatus.Success && records != null)
            {
                Console.WriteLine($"Registros en la base de datos {databaseName}, tabla {tableName}:");
                foreach (var record in records)
                {
                    Console.WriteLine(record);  // Mostrar cada registro
                }
            }

            return (status, records);
        }
    }
}


