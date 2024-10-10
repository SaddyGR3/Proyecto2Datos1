using Entities;
using StoreDataManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryProcessor.Operations
{
    internal class CreateDatabase
    {
        public OperationStatus Execute(string databaseName)
        {
            // Ejemplo de sentencia: "CREATE DATABASE nombreBaseDatos;"

            // Llamar al Store Data Manager para crear la base de datos
            return Store.GetInstance().CreateDatabase(databaseName);
        }
    }
}

