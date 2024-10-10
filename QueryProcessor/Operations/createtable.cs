using Entities;
using StoreDataManager;

namespace QueryProcessor.Operations
{
    internal class CreateTable
    {
        public OperationStatus Execute(string databaseName, string tableName, string sentence)
        {
            // Ejemplo de sentencia: "CREATE TABLE nombreTabla (ID, Nombre, Apellido);"

            // Extraer la parte de las columnas (todo lo que está dentro de los paréntesis)
            var columnsPart = sentence.Split('(')[1].Split(')')[0]?.Trim();
            if (string.IsNullOrEmpty(columnsPart))
            {
                Console.WriteLine("Error: No se encontraron las columnas.");
                return OperationStatus.Error;
            }

            // Separar las columnas y eliminar cualquier espacio innecesario
            var columns = columnsPart.Split(',')
                                     .Select(c => c.Trim())  // Eliminar espacios innecesarios
                                     .ToList();

            // Crear la tabla utilizando el Store Data Manager (sin tipos de datos)
            return Store.GetInstance().CreateTable(databaseName, tableName, columns);
        }
    }
}

