using Entities;
using StoreDataManager;

namespace QueryProcessor.Operations
{
    internal class Select
    {
        public (OperationStatus, List<string>) Execute(string databaseName, string tableName, string sentence)
        {
            //Se asegura de que la sentencia comience con SELECT
            if (!sentence.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Error: La sentencia no comienza con SELECT.");
                return (OperationStatus.Error, null);
            }

            //Llama al Store Data Manager 
            var (status, records) = Store.GetInstance().SelectFromTable(databaseName, tableName);

            if (status == OperationStatus.Success && records != null)
            {
                Console.WriteLine($"Registros en la base de datos {databaseName}, tabla {tableName}:");
                foreach (var record in records)
                {
                    Console.WriteLine(record);  //Muestra cada registro
                }
            }

            return (status, records);
        }
    }
}


