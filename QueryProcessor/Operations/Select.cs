using Entities;
using StoreDataManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryProcessor.Operations
{
    internal class Select
    {
        public (OperationStatus, List<string>) Execute(string sentence)
        {
            // Ejemplo: "SELECT * FROM tableName WHERE ID = 1;"
            var whereClause = sentence.Contains("WHERE") ? sentence.Split("WHERE")[1].Trim(' ', ';') : null;
            int? id = null;

            if (!string.IsNullOrEmpty(whereClause))
            {
                id = int.Parse(whereClause.Split('=')[1].Trim());
            }

            // Llamamos al Store para hacer la búsqueda
            return Store.GetInstance().Select(id);
        }
    }
}
