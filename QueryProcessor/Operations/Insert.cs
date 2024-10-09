using Entities;
using StoreDataManager;

namespace QueryProcessor.Operations
{
    internal class Insert
    {
        public OperationStatus Execute(string sentence)
        {
            // Ejemplo de sentencia: "INSERT INTO Estudiantes VALUES (1, 'Pizza', 'Lunes');"

            // Extraer el nombre de la tabla entre "INSERT INTO" y "VALUES"
            var tableName = sentence.Split("INTO")[1].Split("VALUES")[0]?.Trim();
            if (string.IsNullOrEmpty(tableName))
            {
                Console.WriteLine("Error: No se encontró el nombre de la tabla.");
                return OperationStatus.Error;
            }

            // Extraer la sección de valores (todo lo que está entre los paréntesis)
            var valuesSection = sentence.Split("VALUES")[1]?.Trim(' ', ';', '(', ')');
            if (string.IsNullOrEmpty(valuesSection))
            {
                Console.WriteLine("Error: No se encontraron los valores.");
                return OperationStatus.Error;
            }

            var values = valuesSection.Split(',');
            if (values.Length != 3)
            {
                Console.WriteLine("Error: Se esperan 3 valores (ID, Comida, Día).");
                return OperationStatus.Error; // Se esperan 3 valores
            }

            // Limpiar los valores
            int id;
            if (!int.TryParse(values[0].Trim(), out id))
            {
                Console.WriteLine("Error: El ID debe ser un número.");
                return OperationStatus.Error;
            }

            var c1 = values[1].Trim(' ', '\''); //c1 es columna 1
            var c2 = values[2].Trim(' ', '\''); //c2 es columna 2

            // Llamar al Store Data Manager para insertar en la tabla especificada
            return Store.GetInstance().InsertIntoTable(tableName, id, c1, c2);
        }
    }
}

