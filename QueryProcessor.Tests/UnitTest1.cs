
using QueryProcessor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryProcessor.Exceptions;
using QueryProcessor.Parser;    

namespace QueryProcessor.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestClass]
        public class SQLParserTests
        {
            [TestMethod]
            public void Parse_CreateTable_ValidInput_ReturnsParsedQuery()
            {
                // Arrange
                string createTableQuery = "CREATE TABLE Estudiantes (Nombre VARCHAR(50), Apellidos VARCHAR(100), Edad INT)";

                // Act
                ParsedQuery result = SQLParser.Parse(createTableQuery);

                // Assert
                Assert.AreEqual(CommandType.CreateTable, result.CommandType);
                Assert.AreEqual("Estudiantes", result.TableName);
                Assert.AreEqual(3, result.Columns.Count);
                Assert.AreEqual("VARCHAR(50)", result.Columns["Nombre"]);
                Assert.AreEqual("VARCHAR(100)", result.Columns["Apellidos"]);
                Assert.AreEqual("INT", result.Columns["Edad"]);
            }

            [TestMethod]
            public void Parse_InsertInto_ValidInput_ReturnsParsedQuery()
            {
                // Arrange
                string insertQuery = "INSERT INTO Estudiantes (Nombre, Apellidos, Edad) VALUES ('Juan', 'Pérez', 25)";

                // Act
                ParsedQuery result = SQLParser.Parse(insertQuery);

                // Assert
                Assert.AreEqual(CommandType.Insert, result.CommandType);
                Assert.AreEqual("Estudiantes", result.TableName);
                Assert.AreEqual(3, result.Values.Count);
                Assert.AreEqual("Juan", result.Values["Nombre"]);
                Assert.AreEqual("Pérez", result.Values["Apellidos"]);
                Assert.AreEqual("25", result.Values["Edad"]); // Valores tratados como strings en este caso
            }

            [TestMethod]
            public void Parse_Select_ValidInput_ReturnsParsedQuery()
            {
                // Arrange
                string selectQuery = "SELECT * FROM Estudiantes";

                // Act
                ParsedQuery result = SQLParser.Parse(selectQuery);

                // Assert
                Assert.AreEqual(CommandType.Select, result.CommandType);
                Assert.AreEqual("Estudiantes", result.TableName);
            }

            [TestMethod]
            [ExpectedException(typeof(SQLParserException))]
            public void Parse_InvalidCommand_ThrowsSQLParserException()
            {
                // Arrange
                string invalidQuery = "DROP TABLE Estudiantes";

                // Act
                SQLParser.Parse(invalidQuery);

                // Assert handled by ExpectedException
            }

            [TestMethod]
            [ExpectedException(typeof(SQLParserException))]
            public void Parse_CreateTable_InvalidSyntax_ThrowsSQLParserException()
            {
                // Arrange
                string invalidCreateTableQuery = "CREATE TABLE Estudiantes Nombre VARCHAR(50), Apellidos VARCHAR(100), Edad INT)";

                // Act
                SQLParser.Parse(invalidCreateTableQuery);

                // Assert handled by ExpectedException
            }

            [TestMethod]
            [ExpectedException(typeof(SQLParserException))]
            public void Parse_InsertInto_InvalidColumnCount_ThrowsSQLParserException()
            {
                // Arrange
                string invalidInsertQuery = "INSERT INTO Estudiantes (Nombre, Apellidos) VALUES ('Juan')";

                // Act
                SQLParser.Parse(invalidInsertQuery);

                // Assert handled by ExpectedException
            }
        }
    }
}