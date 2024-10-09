using Entities;
using StoreDataManager;

namespace QueryProcessor.Operations
{
    internal class Delete
    {
        public OperationStatus Execute(string sentence)
        {
            // Ejemplo: "DELETE FROM tableName WHERE ID = 1;"
            var whereClause = sentence.Split("WHERE")[1]?.Trim(' ', ';');
            if (string.IsNullOrEmpty(whereClause))
            {
                return OperationStatus.Error;
            }

            // Extraemos la condición ID = X
            var id = int.Parse(whereClause.Split('=')[1].Trim());

            // Llamamos al Store Data Manager para realizar el borrado
            return Store.GetInstance().Delete(id);
        }
    }
}
