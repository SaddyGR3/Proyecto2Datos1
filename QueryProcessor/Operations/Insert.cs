using Entities;
using StoreDataManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryProcessor.Operations
{
    internal class Insert
    {
        public OperationStatus Execute(string sentence)
        {
            // Aquí se espera que la sentencia sea algo como "INSERT INTO tableName VALUES (1, 'Pizza', 'Lunes')"
            var valuesSection = sentence.Split("VALUES")[1]?.Trim(' ', ';', '(', ')');
            if (string.IsNullOrEmpty(valuesSection))
            {
                return OperationStatus.Error;
            }

            var values = valuesSection.Split(',');
            if (values.Length != 3)
            {
                return OperationStatus.Error; // Se esperan 3 valores (ID, Comida, Día)
            }

            // Limpiamos los valores
            var id = int.Parse(values[0].Trim());
            var comida = values[1].Trim(' ', '\'');
            var dia = values[2].Trim(' ', '\'');

            // Llamamos al Store Data Manager para realizar la inserción
            return Store.GetInstance().Insert(id, comida, dia);
        }
    }
}
