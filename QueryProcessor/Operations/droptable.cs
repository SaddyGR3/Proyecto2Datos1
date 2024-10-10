using Entities;
using StoreDataManager;

namespace QueryProcessor.Operations
{
    internal class DropTable
    {
        public OperationStatus Execute(string sentence)
        {
            // Ejemplo de sentencia: "DROP TABLE tableName;"
            
            var tableName = sentence.Split("TABLE")[1].Trim(';', ' ', '(', ')');
            Console.WriteLine($"Intentando eliminar la tabla: {tableName}");
            if (string.IsNullOrEmpty(tableName))
            {
                Console.WriteLine("Error: No se encontró el nombre de la tabla.");
                return OperationStatus.Error;
            }

            // Llamar al Store Data Manager para eliminar la tabla especificada
            return Store.GetInstance().DropTable(tableName);
        }
    }
}
