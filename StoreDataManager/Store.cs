using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace StoreDataManager
{
    public enum OperationStatus
    {
        Success,
        DatabaseAlreadyExists,
        DatabaseNotFound,
        TableAlreadyExists,
        Error
    }

    public class ColumnDefinition
    {
        public string Name { get; set; }
        public string Type { get; set; }

        public ColumnDefinition(string name, string type)
        {
            this.Name = name;
            this.Type = type;
        }
    }

    public sealed class Store
    {
        private static Store? instance = null;
        private static readonly object _lock = new object();

        private const string DatabaseBasePath = @"C:\TinySql\";
        private const string DataPath = $@"{DatabaseBasePath}\Data";
        private const string SystemCatalogPath = $@"{DataPath}\SystemCatalog";
        private const string SystemDatabasesFile = $@"{SystemCatalogPath}\SystemDatabases.table";
        private const string SystemTablesFile = $@"{SystemCatalogPath}\SystemTables.table";

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

        public Store()
        {
            this.InitializeSystemCatalog();
        }

        private void InitializeSystemCatalog()
        {
            Directory.CreateDirectory(SystemCatalogPath);
        }

        public OperationStatus CreateDatabase(string databaseName)
        {
            var databasePath = $@"{DataPath}\{databaseName}";
            if (!Directory.Exists(databasePath))
            {
                Directory.CreateDirectory(databasePath);

                // Actualiza SystemCatalog
                using (FileStream stream = File.Open(SystemDatabasesFile, FileMode.Append))
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(databaseName);
                }

                return OperationStatus.Success;
            }
            return OperationStatus.DatabaseAlreadyExists;
        }
        public OperationStatus InsertIntoTable(string tableName, Dictionary<string, object> rowData) //Este metodo inserta una fila en una tabla especifica.
        {
            var tablePath = $@"{DataPath}\TESTDB\{tableName}.Table"; //falta agregar la base de datos en la que se va a insertar la fila.

            if (!File.Exists(tablePath))//Si la tabla no existe, se devuelve un error.
            {
                return OperationStatus.Error;
            }

            try
            {
                using (FileStream stream = File.Open(tablePath, FileMode.Append)) //Se abre el archivo binario de la tabla en modo Append.
                using (BinaryWriter writer = new BinaryWriter(stream))//Se crea un BinaryWriter para escribir en el archivo.
                {
                    //Escribe cada columna de la fila en el archivo.
                    foreach (var column in rowData)
                    {
                        if (column.Value is int)
                        {
                            writer.Write((int)column.Value);
                        }
                        else if (column.Value is string)
                        {
                            string value = (string)column.Value;

                            if (column.Key == "Nombre")
                            {
                                value = value.PadRight(30); //Se asegura de que el string tenga 30 caracteres.
                            }
                            else if (column.Key == "Apellidos")
                            {
                                value = value.PadRight(50); //Se asegura de que el string tenga 50 caracteres.
                            }

                            writer.Write(value);
                        }
                    }
                }
                return OperationStatus.Success;
            }
            catch (Exception ex)
            {
                // Manejar el error y devolver el estado de error
                return OperationStatus.Error;
            }
        }

        public OperationStatus CreateTable(string databaseName, string tableName, List<ColumnDefinition> columns)
        {
            var databasePath = $@"{DataPath}\{databaseName}";
            if (!Directory.Exists(databasePath))
            {
                return OperationStatus.DatabaseNotFound;
            }

            var tablePath = $@"{databasePath}\{tableName}.Table";
            using (FileStream stream = File.Open(tablePath, FileMode.OpenOrCreate))
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                // Escribir el esquema de la tabla en el archivo
                foreach (var column in columns)
                {
                    writer.Write(column.Name);
                    writer.Write(column.Type);
                }
            }

            // Actualiza el SystemCatalog
            using (FileStream stream = File.Open(SystemTablesFile, FileMode.Append))
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(databaseName);
                writer.Write(tableName);
            }

            return OperationStatus.Success;
        }
    }
}
