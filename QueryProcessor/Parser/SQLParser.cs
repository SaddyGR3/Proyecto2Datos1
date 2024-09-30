using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QueryProcessor.Exceptions;

namespace QueryProcessor.Parser
{
    public class SQLParser
    {
        public static ParsedQuery Parse(string sentence)
        {
            if (sentence.StartsWith("CREATE TABLE"))
            {
                // Lógica para analizar la sentencia CREATE TABLE
                return ParseCreateTable(sentence);
            }
            else if (sentence.StartsWith("INSERT INTO"))
            {
                // Lógica para analizar la sentencia INSERT
                return ParseInsert(sentence);
            }
            else if (sentence.StartsWith("SELECT"))
            {
                // Lógica para analizar la sentencia SELECT
                return ParseSelect(sentence);
            }
            else
            {
                throw new SQLParserException("Unknown SQL command.");
            }
        }

        public static ParsedQuery ParseCreateTable(string sentence)
        {
            // Ejemplo de sentencia: "CREATE TABLE Estudiantes (Nombre VARCHAR(50), Apellidos VARCHAR(100), Edad INT)"

            // Eliminamos el "CREATE TABLE" del comienzo y limpiamos la sentencia
            string withoutCreateTable = sentence.Substring("CREATE TABLE".Length).Trim(); //este .Trim() elimina los espacios en blanco al principio y al final de un string.

            // Separamos el nombre de la tabla de las columnas
            int openParenIndex = withoutCreateTable.IndexOf('(');
            int closeParenIndex = withoutCreateTable.LastIndexOf(')');

            if (openParenIndex == -1 || closeParenIndex == -1)
            {
                throw new SQLParserException("Syntax error in CREATE TABLE statement.");
            }

            // Extraemos el nombre de la tabla
            string tableName = withoutCreateTable.Substring(0, openParenIndex).Trim();

            // Extraemos las columnas dentro de los paréntesis
            string columnsPart = withoutCreateTable.Substring(openParenIndex + 1, closeParenIndex - openParenIndex - 1).Trim();

            // Creamos el diccionario para las columnas
            var columns = new Dictionary<string, object>();

            // Dividimos por comas para obtener cada columna y su tipo
            var columnDefinitions = columnsPart.Split(',');

            foreach (var columnDefinition in columnDefinitions)
            {
                // Separamos el nombre de la columna del tipo (e.g., "Nombre VARCHAR(50)")
                var parts = columnDefinition.Trim().Split(' ');
                if (parts.Length < 2)
                {
                    throw new SQLParserException("Syntax error in column definition.");
                }

                string columnName = parts[0].Trim();
                string columnType = parts[1].Trim();

                // Agregamos la columna al diccionario
                columns[columnName] = columnType;
            }

            // Retornamos el objeto ParsedQuery
            return new ParsedQuery
            {
                CommandType = CommandType.CreateTable,
                TableName = tableName,
                Columns = columns
            };
        }

        public static ParsedQuery ParseInsert(string sentence)
        {
            // Ejemplo de sentencia: "INSERT INTO Estudiantes (Nombre, Apellidos, Edad) VALUES ('Juan', 'Pérez', 25)"

            // Limpiamos la sentencia eliminando "INSERT INTO"
            string withoutInsertInto = sentence.Substring("INSERT INTO".Length).Trim();

            // Extraemos el nombre de la tabla antes del paréntesis
            int openParenIndex = withoutInsertInto.IndexOf('(');
            int closeParenIndex = withoutInsertInto.IndexOf(')', openParenIndex);

            if (openParenIndex == -1 || closeParenIndex == -1)
            {
                throw new SQLParserException("Syntax error in INSERT INTO statement.");
            }

            string tableName = withoutInsertInto.Substring(0, openParenIndex).Trim();

            // Extraemos las columnas
            string columnsPart = withoutInsertInto.Substring(openParenIndex + 1, closeParenIndex - openParenIndex - 1).Trim();
            var columns = columnsPart.Split(',').Select(c => c.Trim()).ToList();

            // Extraemos los valores
            int valuesIndex = withoutInsertInto.IndexOf("VALUES", closeParenIndex);
            if (valuesIndex == -1)
            {
                throw new SQLParserException("Missing VALUES keyword in INSERT INTO statement.");
            }

            string valuesPart = withoutInsertInto.Substring(valuesIndex + "VALUES".Length).Trim();
            valuesPart = valuesPart.Trim('(', ')'); // Limpiar paréntesis
            var values = valuesPart.Split(',').Select(v => v.Trim('\'', ' ')).ToList();

            if (columns.Count != values.Count)
            {
                throw new SQLParserException("Number of columns and values do not match.");
            }

            // Creamos el diccionario de valores
            var rowData = new Dictionary<string, object>();
            for (int i = 0; i < columns.Count; i++)
            {
                rowData[columns[i]] = values[i];
            }

            return new ParsedQuery
            {
                CommandType = CommandType.Insert,
                TableName = tableName,
                Values = rowData
            };
        }

        public static ParsedQuery ParseSelect(string sentence)
        {
            // Ejemplo: "SELECT * FROM Estudiantes"

            string withoutSelect = sentence.Substring("SELECT".Length).Trim();
            string[] parts = withoutSelect.Split(new string[] { "FROM" }, StringSplitOptions.None);

            if (parts.Length != 2)
            {
                throw new SQLParserException("Syntax error in SELECT statement.");
            }

            string tableName = parts[1].Trim(); // Obtenemos el nombre de la tabla

            return new ParsedQuery
            {
                CommandType = CommandType.Select,
                TableName = tableName
            };
        }
    }

    public class ParsedQuery
    {
        public CommandType CommandType { get; set; }
        public string TableName { get; set; }
        public Dictionary<string, object> Columns { get; set; }
        public Dictionary<string, object> Values { get; set; }
    }

    public enum CommandType
    {
        CreateTable,
        Insert,
        Select
    }
}

