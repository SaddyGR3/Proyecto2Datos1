using Entities;
using StoreDataManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryProcessor.Operations
{
    internal class Update
    {
        public OperationStatus Execute(string databaseName, string tableName, string sentence)
        {
            // Verifica que la sentencia tenga una cláusula SET y WHERE
            if (!sentence.Contains("SET", StringComparison.OrdinalIgnoreCase) ||
                !sentence.Contains("WHERE", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Error: La sentencia debe contener las cláusulas SET y WHERE.");
                return OperationStatus.Error;
            }

            // Extrae la cláusula SET (columna = valor, ...)
            var setClause = sentence.Split("SET")[1]?.Split("WHERE")[0]?.Trim();
            if (string.IsNullOrEmpty(setClause))
            {
                Console.WriteLine("Error: No se encontraron los valores a actualizar.");
                return OperationStatus.Error;
            }

            // Divide las asignaciones columna = valor
            var assignments = setClause.Split(',')
                                       .Select(a => a.Split('='))
                                       .ToDictionary(parts => parts[0].Trim(), parts => parts[1].Trim(' ', '\'', '"'));

            // Extrae la condición del WHERE (ID = X)
            var whereClause = sentence.Split("WHERE")[1]?.Trim(' ', ';');
            if (string.IsNullOrEmpty(whereClause))
            {
                Console.WriteLine("Error: No se encontró la condición para actualizar.");
                return OperationStatus.Error;
            }

            // Verifica si el WHERE usa ID como condición
            var idCondition = whereClause.Split('=')[1].Trim();
            if (!int.TryParse(idCondition, out int id))
            {
                Console.WriteLine("Error: El ID especificado no es válido.");
                return OperationStatus.Error;
            }

            try
            {
                // Llama al Store Data Manager para actualizar la tabla con los nuevos valores
                return Store.GetInstance().UpdateTable(databaseName, tableName, id, assignments);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar la tabla: {ex.Message}");
                return OperationStatus.Error;
            }
        }
    }
}
