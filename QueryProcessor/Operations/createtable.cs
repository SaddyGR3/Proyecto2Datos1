using Entities;
using StoreDataManager;
using System.Text.RegularExpressions;

namespace QueryProcessor.Operations
{
    internal class CreateTable
    {
        public OperationStatus Execute(string databaseName, string tableName, string sentence)
        {
            try
            {
                Console.WriteLine($"Procesando CREATE TABLE para la base de datos: {databaseName}, tabla: {tableName}");

                var match = Regex.Match(sentence, @"CREATE\s+TABLE\s+\w+\s*\((.*)\)", RegexOptions.IgnoreCase);
                if (!match.Success)
                {
                    Console.WriteLine("Error: No se encontraron las columnas.");
                    return OperationStatus.Error;
                }

                var columnsPart = match.Groups[1].Value?.Trim();
                if (string.IsNullOrEmpty(columnsPart))
                {
                    Console.WriteLine("Error: No se encontraron las columnas.");
                    return OperationStatus.Error;
                }

                Console.WriteLine($"Sección de columnas extraída: {columnsPart}");

                // Separa las columnas correctamente, asegurando que no se pierdan columnas con paréntesis
                var columns = columnsPart.Split(',')
                                         .Select(c => c.Trim())
                                         .ToList();

                // Crea lista de ColumnDefinition, esta vez manejando correctamente el tamaño de VARCHAR
                var columnDefinitions = new List<ColumnDefinition>();

                foreach (var column in columns)
                {
                    var columnParts = column.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    if (columnParts.Length >= 2)
                    {
                        var columnName = columnParts[0].Trim();  // Nombre de la columna
                        var dataTypePart = columnParts[1].Trim();  // Tipo de dato con posible paréntesis

                        // Intentar detectar si hay un tamaño especificado entre paréntesis (p.ej. VARCHAR(30))
                        var dataTypeWithoutLength = RemoveParentheses(dataTypePart);
                        int? maxLength = ExtractLength(dataTypePart);

                        // Mapear a enum DataType (simplificado)
                        var dataTypeEnum = dataTypeWithoutLength.Equals("INT", StringComparison.OrdinalIgnoreCase) ? DataType.Integer
                                            : dataTypeWithoutLength.Equals("DATETIME", StringComparison.OrdinalIgnoreCase) ? DataType.Datetime
                                            : DataType.Varchar;

                        columnDefinitions.Add(new ColumnDefinition
                        {
                            ColumnName = columnName,
                            DataType = dataTypeEnum,
                            MaxLength = maxLength
                        });

                        // Mensaje de depuración para cada columna
                        Console.WriteLine($"Columna procesada: Nombre: {columnName}, Tipo: {dataTypeWithoutLength}, Longitud: {maxLength}, TipoEnum: {dataTypeEnum}");
                    }
                    else
                    {
                        Console.WriteLine($"Error: La columna '{column}' no está correctamente formateada.");
                        return OperationStatus.Error;
                    }
                }

                // Mensaje de depuración antes de llamar al Store
                Console.WriteLine("Columnas procesadas:");
                foreach (var col in columnDefinitions)
                {
                    Console.WriteLine($"Columna: {col.ColumnName}, Tipo: {col.DataType}, Longitud: {col.MaxLength}");
                }

                // Llama al Store para crear la tabla con los nombres de las columnas y los tipos de datos
                var result = Store.GetInstance().CreateTable(databaseName, tableName, columnDefinitions);

                // Mensaje de depuración después de llamar al Store
                Console.WriteLine($"Resultado de la creación de la tabla: {result}");

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al procesar el CREATE TABLE: {ex.Message}");
                return OperationStatus.Error;
            }
        }

        // Método para eliminar cualquier contenido dentro de paréntesis
        private string RemoveParentheses(string input)
        {
            var indexOfParenthesis = input.IndexOf('(');
            return indexOfParenthesis >= 0 ? input.Substring(0, indexOfParenthesis) : input;
        }

        // Método para extraer el tamaño del VARCHAR entre paréntesis
        private int? ExtractLength(string input)
        {
            var start = input.IndexOf('(');
            var end = input.IndexOf(')');
            if (start >= 0 && end > start)
            {
                if (int.TryParse(input.Substring(start + 1, end - start - 1), out int length))
                {
                    return length;
                }
            }
            return null;  // Si no hay paréntesis o no es un número válido, retornar null
        }
    }
}

