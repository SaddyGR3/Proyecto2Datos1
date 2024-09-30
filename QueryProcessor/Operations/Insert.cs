using StoreDataManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace QueryProcessor.Operations
{
    internal class Insert //Al igual que en CreateTable, hay que añadir un método Execute() que reciba el nombre de la tabla y los valores a insertar.
    {
        internal OperationStatus Execute(string tableName, Dictionary<string, object> rowData)
        {
            return Store.GetInstance().InsertIntoTable(tableName, rowData);
        }
    }
}
