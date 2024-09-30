using Entities;
using QueryProcessor.Exceptions;
using QueryProcessor.Operations;
using StoreDataManager;

namespace QueryProcessor
{
    public class SQLQueryProcessor
    {
        public static OperationStatus Execute(string sentence) //Este metodo procesa y ejecuta una sentencia SQL recibida como parámetro.
        {                                                      //Realiza una comprobacion inicial  para saber que tipo de sentencia es y ejecuta la operacion correspondiente.
                                                               //si la sentencia no coincide con ningun parametro, se lanza una excepcion.
            /// The following is example code. Parser should be called
            /// on the sentence to understand and process what is requested
            if (sentence.StartsWith("CREATE TABLE"))   //faltaria agregar el resto de las sentencias SQL y ademas una seccion que se encargue de parsear la sentencia y descomponerla para manejar diferentes tipos de operaciones.
            {                                           //insertar mas clases en la parte de operaciones,como por ejemplo Insert, Update, Delete, etc.
                return new CreateTable().Execute();     //Cada una de estas clases debe tener un método Execute() que lleve a cabo la operación solicitada.
            }   
            if (sentence.StartsWith("SELECT"))
            {
                return new Select().Execute();
            }
            else
            {
                throw new UnknownSQLSentenceException(sentence);
            }
        }
    }
}
