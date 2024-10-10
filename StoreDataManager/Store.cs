using Entities;
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
            InitializeDatabase();
        }

        // Aseguramos que el archivo de la base de datos exista
        private void InitializeDatabase()
        {
            if (!File.Exists(DatabaseFilePath))
            {
                File.Create(DatabaseFilePath).Close();
            }
        }
        public OperationStatus CreateTable(string tableName, List<string> columns)
        {
            // Generar la ruta del archivo que representará la tabla.
            string tableFilePath = $@"C:\TinySql\Data\{tableName}.txt";

            try
            {
                // Verificar si el archivo ya existe (la "tabla" ya existe).
                if (File.Exists(tableFilePath))
                {
                    Console.WriteLine($"La tabla {tableName} ya existe.");
                    return OperationStatus.TableAlreadyExists; // Estado para tabla existente
                }

                // Crear el archivo de la tabla y escribir solo los nombres de las columnas en la primera línea
                using (StreamWriter writer = new StreamWriter(tableFilePath))
                {
                    // Escribir los nombres de las columnas en la primera línea (sin tipos de datos)
                    writer.WriteLine(string.Join(",", columns));
                }

                Console.WriteLine($"Tabla {tableName} creada exitosamente.");
                return OperationStatus.Success; // Estado para creación exitosa
            }
            catch (Exception ex)
            {
                // Capturar errores en caso de fallo
                Console.WriteLine($"Error al crear la tabla: {ex.Message}");
                return OperationStatus.Error; // Estado para errores
            }
        }




        // Método para insertar un nuevo registro
        public OperationStatus InsertIntoTable(string tableName, int id, string c1, string c2)
        {
            try
            {
                // Generar la ruta del archivo correspondiente a la tabla
                string tableFilePath = $@"C:\TinySql\Data\{tableName}.txt";

                // Verificar si la tabla existe (archivo)
                if (!File.Exists(tableFilePath))
                {
                    Console.WriteLine($"Error: La tabla {tableName} no existe.");
                    return OperationStatus.Error;  // Retorna Error si la tabla no existe
                }

                // Formatear el registro como una línea de texto
                string record = $"{id},{c1},{c2}";

                // Insertar el registro en el archivo de texto
                using (StreamWriter writer = new StreamWriter(tableFilePath, append: true))
                {
                    writer.WriteLine(record);
                }

                Console.WriteLine($"Registro insertado correctamente en {tableName}.");
                return OperationStatus.Success;  // Retorna éxito si la inserción fue correcta
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al insertar: {ex.Message}");
                return OperationStatus.Error;  // Retorna Error si ocurre una excepción
            }
        }




        // Método para seleccionar registros, basado en un ID opcional
        public (OperationStatus, List<string>) SelectFromTable(string tableName)
        {
            try
            {
                // Generar la ruta del archivo correspondiente a la tabla
                string tableFilePath = $@"C:\TinySql\Data\{tableName}.txt";

                // Verificar si la tabla existe (archivo)
                if (!File.Exists(tableFilePath))
                {
                    Console.WriteLine($"Error: La tabla {tableName} no existe.");
                    return (OperationStatus.Error, null);  // Retorna Error si la tabla no existe
                }

                // Lista para almacenar los registros
                var records = new List<string>();

                // Leer el archivo de la tabla
                using (StreamReader reader = new StreamReader(tableFilePath))
                {
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        records.Add(line);  // Agregar cada línea al listado
                    }
                }

                return (OperationStatus.Success, records);  // Retorna éxito y los registros leídos
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al seleccionar desde la tabla: {ex.Message}");
                return (OperationStatus.Error, null);  // Retorna Error si ocurre una excepción
            }
        }




        // Método para eliminar un registro basado en el ID
        public OperationStatus DeleteFromTable(string tableName, int id)
        {
            try
            {
                // Generar la ruta del archivo correspondiente a la tabla
                string tableFilePath = $@"C:\TinySql\Data\{tableName}.txt";

                // Verificar si la tabla existe (archivo)
                if (!File.Exists(tableFilePath))
                {
                    Console.WriteLine($"Error: La tabla {tableName} no existe.");
                    return OperationStatus.Error;  // Retorna Error si la tabla no existe
                }

                var tempFile = Path.GetTempFileName();
                bool found = false;

                // Leer el archivo y escribir en un archivo temporal
                using (var reader = new StreamReader(tableFilePath))
                using (var writer = new StreamWriter(tempFile))
                {
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var parts = line.Split(',');

                        if (parts.Length < 1)
                        {
                            writer.WriteLine(line); // Escribir líneas vacías
                            continue;
                        }

                        // Suponemos que el primer elemento es el ID
                        if (int.TryParse(parts[0].Trim(), out int recordId) && recordId == id)
                        {
                            found = true; // Marcamos que encontramos el ID
                        }
                        else
                        {
                            writer.WriteLine(line); // Escribir la línea si no coincide con el ID
                        }
                    }
                }

                // Reemplazar el archivo original por el temporal
                File.Delete(tableFilePath);
                File.Move(tempFile, tableFilePath);

                return found ? OperationStatus.Success : OperationStatus.Error; // Retorna éxito si se eliminó un registro
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el registro: {ex.Message}");
                return OperationStatus.Error;  // Retorna Error si ocurre una excepción
            }
        }
        public OperationStatus DropTable(string tableName)
        {
            try
            {
                // Generar la ruta del archivo correspondiente a la tabla
                string tableFilePath = $@"C:\TinySql\Data\{tableName}.txt";

                // Mensaje de depuración para verificar el archivo
                Console.WriteLine($"Intentando eliminar la tabla: {tableName}");
                Console.WriteLine($"Ruta del archivo: {tableFilePath}");

                // Verificar si el archivo existe
                if (!File.Exists(tableFilePath))
                {
                    Console.WriteLine($"Error: La tabla {tableName} no existe.");
                    return OperationStatus.Error;  // Retorna Error si la tabla no existe
                }

                // Verificar si el archivo está vacío
                FileInfo fileInfo = new FileInfo(tableFilePath);
                if (fileInfo.Length > 0)
                {
                    Console.WriteLine($"Error: La tabla {tableName} no está vacía y no puede ser eliminada.");
                    return OperationStatus.Error;  // Retorna Error si la tabla no está vacía
                }

                // Intentar eliminar el archivo
                try
                {
                    File.Delete(tableFilePath);
                    Console.WriteLine($"Tabla {tableName} eliminada exitosamente.");
                    return OperationStatus.Success;  // Retorna éxito si la eliminación fue correcta
                }
                catch (IOException ioEx)
                {
                    Console.WriteLine($"Error de IO al eliminar la tabla: {ioEx.Message}");
                    return OperationStatus.Error;  // Retorna Error si ocurre una excepción de IO
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error inesperado al eliminar la tabla: {ex.Message}");
                    return OperationStatus.Error;  // Retorna Error si ocurre cualquier otra excepción
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al procesar la tabla: {ex.Message}");
                return OperationStatus.Error;  // Retorna Error si ocurre una excepción
            }
        }





    }
}
