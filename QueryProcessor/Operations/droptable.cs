using Entities;
using StoreDataManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryProcessor.Operations
{
    internal class DropTable
    {
        public OperationStatus Execute(string databaseName, string sentence)
        {
            try
            {
                // Ejemplo de sentencia: "DROP TABLE Estudiantes;"

                // Extrae el nombre de la tabla desde la sentencia SQL
                var tableName = ExtractTableName(sentence);
                if (string.IsNullOrEmpty(tableName))
                {
                    Console.WriteLine("Error: No se especificó un nombre de tabla válido.");
                    return OperationStatus.Error;
                }

                // Llama al Store Data Manager para eliminar la tabla
                var result = Store.GetInstance().DropTable(databaseName, tableName);

                if (result == OperationStatus.Success)
                {
                    Console.WriteLine($"Tabla '{tableName}' eliminada exitosamente.");
                }
                else
                {
                    Console.WriteLine($"Error: No se pudo eliminar la tabla '{tableName}'.");
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al ejecutar DROP TABLE: {ex.Message}");
                return OperationStatus.Error;
            }
        }

        // Método auxiliar para extraer el nombre de la tabla desde la sentencia SQL
        private string ExtractTableName(string sentence)
        {
            // Remueve el prefijo "DROP TABLE" y el ";" al final
            var cleanedSentence = sentence.Replace("DROP TABLE", "", StringComparison.OrdinalIgnoreCase).Trim().TrimEnd(';');
            return cleanedSentence;
        }
    }
}
