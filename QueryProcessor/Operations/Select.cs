using Entities;
using StoreDataManager;

namespace QueryProcessor.Operations
{
    internal class Select
    {
        public (OperationStatus, List<string>) Execute(string sentence)
        {
            // Asegúrate de que la sentencia comienza con SELECT
            if (!sentence.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Error: La sentencia no comienza con SELECT.");
                return (OperationStatus.Error, null);
            }

            // Extraer el nombre de la tabla después de FROM
            var tableName = sentence.Split("FROM")[1]?.Trim(';', ' ', '(', ')');
            if (string.IsNullOrEmpty(tableName))
            {
                Console.WriteLine("Error: No se encontró el nombre de la tabla.");
                return (OperationStatus.Error, null);
            }

            // Llamar al Store Data Manager para seleccionar desde la tabla especificada
            var (status, records) = Store.GetInstance().SelectFromTable(tableName);

            if (status == OperationStatus.Success && records != null)
            {
                Console.WriteLine($"Registros en la tabla {tableName}:");
                foreach (var record in records)
                {
                    Console.WriteLine(record);  // Mostrar cada registro
                }
            }

            return (status, records);
        }
    }
}


