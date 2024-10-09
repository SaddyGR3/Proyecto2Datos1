using Entities;
using StoreDataManager;

namespace QueryProcessor.Operations
{
    internal class Delete
    {
        public OperationStatus Execute(string sentence)
        {
            // Ejemplo de sentencia: "DELETE FROM tableName WHERE ID = 1;"

            var tableName = sentence.Split("FROM")[1].Split("WHERE")[0].Trim();
            if (string.IsNullOrEmpty(tableName))
            {
                Console.WriteLine("Error: No se encontró el nombre de la tabla.");
                return OperationStatus.Error;
            }

            // Extraer la condición (ID = X)
            var whereClause = sentence.Split("WHERE")[1]?.Trim(' ', ';');
            if (string.IsNullOrEmpty(whereClause))
            {
                Console.WriteLine("Error: No se encontró la condición para eliminar.");
                return OperationStatus.Error;
            }

            // Extraemos el ID de la condición
            var idString = whereClause.Split('=')[1].Trim();
            if (!int.TryParse(idString, out int id))
            {
                Console.WriteLine("Error: El ID especificado no es válido.");
                return OperationStatus.Error;
            }

            // Llamar al Store Data Manager para realizar el borrado
            return Store.GetInstance().DeleteFromTable(tableName, id);
        }
    }
}
