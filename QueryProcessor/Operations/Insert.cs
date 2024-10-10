using Entities;
using StoreDataManager;

namespace QueryProcessor.Operations
{
    internal class Insert
    {
        public OperationStatus Execute(string databaseName, string tableName, string sentence)
        {
            // Ejemplo de sentencia: "INSERT INTO Estudiantes VALUES (1, 'Pizza', 'Lunes', 'Viernes', '2024-10-01 10:30:00');"

            // Extraer la sección de valores (todo lo que está entre los paréntesis)
            var valuesSection = sentence.Split("VALUES")[1]?.Trim(' ', ';', '(', ')');
            if (string.IsNullOrEmpty(valuesSection))
            {
                Console.WriteLine("Error: No se encontraron los valores.");
                return OperationStatus.Error;
            }

            // Dividir los valores, asegurando que se eliminen las comillas simples o dobles alrededor de las cadenas y la fecha
            var values = valuesSection.Split(',')
                                      .Select(v => v.Trim(' ', '\'', '"'))  // Eliminar comillas adicionales
                                      .ToList();

            // Validar que se reciban 5 valores (ID, varchar1, varchar2, varchar3, datetime)
            if (values.Count != 5)
            {
                Console.WriteLine("Error: Se esperan 5 valores (ID, 3 cadenas de texto y una fecha).");
                return OperationStatus.Error;
            }

            try
            {
                // Procesar cada valor
                int id = int.Parse(values[0]); // ID
                var c1 = values[1];            // Primer varchar
                var c2 = values[2];            // Segundo varchar
                var c3 = values[3];            // Tercer varchar

                // Parsear el valor datetime
                DateTime dateValue = DateTime.Parse(values[4]);

                // Llamar al Store Data Manager para insertar en la tabla
                return Store.GetInstance().InsertIntoTable(databaseName, tableName, id, c1, c2, c3, dateValue);
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Error al convertir los valores: {ex.Message}");
                return OperationStatus.Error;
            }
        }
    }
}
