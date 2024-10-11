using Entities;
using StoreDataManager;

namespace QueryProcessor.Operations
{
    internal class Delete
    {
        public OperationStatus Execute(string databaseName,string tableName, string sentence)
        {
            // Ejemplo de sentencia: "DELETE FROM tableName WHERE ID = 1;"

            // Extrae la condición (ID = X)
            var whereClause = sentence.Split("WHERE")[1]?.Trim(' ', ';');
            if (string.IsNullOrEmpty(whereClause))
            {
                Console.WriteLine("Error: No se encontró la condición para eliminar.");
                return OperationStatus.Error;
            }

            // Extrae la cndicion del ID
            var idString = whereClause.Split('=')[1].Trim();
            if (!int.TryParse(idString, out int id))
            {
                Console.WriteLine("Error: El ID especificado no es válido.");
                return OperationStatus.Error;
            }

            //Llama al Store Data Manager para realizar el borrado
            return Store.GetInstance().DeleteFromTable(databaseName, tableName, id);
        }
    }
}
