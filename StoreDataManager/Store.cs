using Entities;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace StoreDataManager
{
    public sealed class Store
    {
        private static Store? instance = null;
        private static readonly object _lock = new object();

        // Ruta de la base de datos que será un archivo .txt
        private const string DatabaseFilePath = @"C:\TinySql\Data\comidas_db.txt";

        // Ruta base para todas las bases de datos
        private const string BaseDirectoryPath = @"C:\TinySql\Data\";
        // Singleton para asegurar que haya una única instancia de Store
        public static Store GetInstance()
        {
            lock (_lock)
            {
                if (instance == null)
                {
                    instance = new Store();
                }
                return instance;
            }
        }

        // Constructor que asegura la inicialización de la "base de datos"
        public Store()
        {
            InitializeBaseDirectory();
        }

        // Asegura que el directorio base exista
        private void InitializeBaseDirectory()
        {
            if (!Directory.Exists(BaseDirectoryPath))
            {
                Directory.CreateDirectory(BaseDirectoryPath);
            }
        }
        // Metodo para crear una base de datos (directorio)
        public OperationStatus CreateDatabase(string databaseName)
        {
            try
            {
                string databaseDirectoryPath = Path.Combine(BaseDirectoryPath, databaseName);

                // Verifica si ya existe una base de datos con ese nombre
                if (Directory.Exists(databaseDirectoryPath))
                {
                    Console.WriteLine($"La base de datos '{databaseName}' ya existe.");
                    return OperationStatus.DatabaseAlreadyExists;
                }

                // Crea el directorio de la base de datos
                Directory.CreateDirectory(databaseDirectoryPath);
                Console.WriteLine($"Base de datos '{databaseName}' creada exitosamente.");
                return OperationStatus.Success;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear la base de datos: {ex.Message}");
                return OperationStatus.Error;
            }
        }

        public OperationStatus CreateTable(string databaseName, string tableName, List<ColumnDefinition> columns)
        {
            try
            {
                string databaseDirectoryPath = Path.Combine(BaseDirectoryPath, databaseName);

                if (!Directory.Exists(databaseDirectoryPath))
                {
                    Console.WriteLine($"Error: La base de datos '{databaseName}' no existe.");
                    return OperationStatus.DatabaseNotFound;
                }

                string tableFilePath = Path.Combine(databaseDirectoryPath, $"{tableName}.txt");

                if (File.Exists(tableFilePath))
                {
                    Console.WriteLine($"La tabla {tableName} ya existe en la base de datos {databaseName}.");
                    return OperationStatus.TableAlreadyExists;
                }

                // Crea el archivo y escribir la cabecera (nombres de las columnas)
                using (StreamWriter writer = new StreamWriter(tableFilePath))
                {
                    // Extrae solo los nombres de las columnas
                    var columnNames = columns.Select(col => col.ColumnName);

                    // Escribe los nombres de las columnas como la primera línea del archivo
                    writer.WriteLine(string.Join(",", columnNames));
                }

                Console.WriteLine($"Tabla {tableName} creada exitosamente en la base de datos {databaseName}.");
                return OperationStatus.Success;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear la tabla: {ex.Message}");
                return OperationStatus.Error;
            }
        }

        // Método para insertar un nuevo registro en una tabla dentro de una base de datos
        public OperationStatus InsertIntoTable(string databaseName, string tableName, int id, string c1, string c2, string c3, DateTime dateValue)
        {
            try
            {
                string tableFilePath = GetTableFilePath(databaseName, tableName);
                if (tableFilePath == null) return OperationStatus.TableNotFound;

                // Formatea el registro con los cinco valores
                string record = $"{id},{c1},{c2},{c3},{dateValue:yyyy-MM-dd HH:mm:ss}";

                // Inserta el registro en el archivo de texto
                using (StreamWriter writer = new StreamWriter(tableFilePath, append: true))
                {
                    writer.WriteLine(record);
                }

                Console.WriteLine($"Registro insertado correctamente en la tabla {tableName} de la base de datos {databaseName}.");
                return OperationStatus.Success;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al insertar: {ex.Message}");
                return OperationStatus.Error;
            }
        }


        // Metodo para seleccionar todos los registros de una tabla
        public (OperationStatus, List<string>) SelectFromTable(string databaseName, string tableName)
        {
            try
            {
                string tableFilePath = GetTableFilePath(databaseName, tableName);
                if (tableFilePath == null) return (OperationStatus.TableNotFound, null);

                var records = new List<string>();

                using (StreamReader reader = new StreamReader(tableFilePath))
                {
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        records.Add(line);
                    }
                }

                return (OperationStatus.Success, records);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al seleccionar registros: {ex.Message}");
                return (OperationStatus.Error, null);
            }
        }


        // Método para eliminar un registro por ID
        public OperationStatus DeleteFromTable(string databaseName, string tableName, int id)
        {
            try
            {
                string tableFilePath = GetTableFilePath(databaseName, tableName);
                if (tableFilePath == null) return OperationStatus.TableNotFound;

                var tempFile = Path.GetTempFileName();
                bool found = false;

                using (var reader = new StreamReader(tableFilePath))
                using (var writer = new StreamWriter(tempFile))
                {
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var parts = line.Split(',');

                        if (int.TryParse(parts[0].Trim(), out int recordId) && recordId == id)
                        {
                            found = true;
                        }
                        else
                        {
                            writer.WriteLine(line);
                        }
                    }
                }

                File.Delete(tableFilePath);
                File.Move(tempFile, tableFilePath);

                return found ? OperationStatus.Success : OperationStatus.Error;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el registro: {ex.Message}");
                return OperationStatus.Error;
            }
        }
        // Método auxiliar para obtener la ruta completa del archivo de una tabla
        private string? GetTableFilePath(string databaseName, string tableName)
        {
            string databaseDirectoryPath = Path.Combine(BaseDirectoryPath, databaseName);

            // Verifica si la base de datos existe
            if (!Directory.Exists(databaseDirectoryPath))
            {
                Console.WriteLine($"Error: La base de datos '{databaseName}' no existe.");
                return null;
            }

            string tableFilePath = Path.Combine(databaseDirectoryPath, $"{tableName}.txt");

            // Verifica si la tabla existe
            if (!File.Exists(tableFilePath))
            {
                Console.WriteLine($"Error: La tabla '{tableName}' no existe en la base de datos '{databaseName}'.");
                return null;
            }

            return tableFilePath;
        }
    }
}

