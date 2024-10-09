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
        public OperationStatus Insert(int id, string comida, string dia)
        {
            try
            {
                // Formateamos el registro como una línea de texto
                string record = $"{id},{comida},{dia}";

                // Insertamos el registro en el archivo de texto
                using (StreamWriter writer = new StreamWriter(DatabaseFilePath, append: true))
                {
                    writer.WriteLine(record);
                }

                return OperationStatus.Success;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al insertar: {ex.Message}");
                return OperationStatus.Error;
            }
        }

        // Método para seleccionar registros, basado en un ID opcional
        public (OperationStatus, List<string>) Select(int? id = null)
        {
            try
            {
                var results = new List<string>();
                using (StreamReader reader = new StreamReader(DatabaseFilePath))
                {
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        // Cada línea es un registro en formato "id,comida,dia"
                        var parts = line.Split(',');
                        if (parts.Length != 3) continue;

                        int recordId = int.Parse(parts[0].Trim());

                        // Si se especifica un ID, solo seleccionamos el registro con ese ID
                        if (id == null || recordId == id)
                        {
                            results.Add(line);
                        }
                    }
                }

                return (OperationStatus.Success, results);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al seleccionar: {ex.Message}");
                return (OperationStatus.Error, null);
            }
        }

        // Método para eliminar un registro basado en el ID
        public OperationStatus Delete(int id)
        {
            try
            {
                var tempFile = Path.GetTempFileName();
                var found = false;

                using (var reader = new StreamReader(DatabaseFilePath))
                using (var writer = new StreamWriter(tempFile))
                {
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var parts = line.Split(',');
                        if (parts.Length != 3) continue;

                        int recordId = int.Parse(parts[0].Trim());

                        // Si el ID no coincide, copiamos la línea al archivo temporal
                        if (recordId != id)
                        {
                            writer.WriteLine(line);
                        }
                        else
                        {
                            found = true;
                        }
                    }
                }

                // Reemplazamos el archivo original por el archivo temporal
                File.Delete(DatabaseFilePath);
                File.Move(tempFile, DatabaseFilePath);

                return found ? OperationStatus.Success : OperationStatus.Error;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar: {ex.Message}");
                return OperationStatus.Error;
            }
        }
    }
}
