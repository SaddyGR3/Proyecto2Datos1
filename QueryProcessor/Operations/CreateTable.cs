using Entities;
using StoreDataManager;

namespace QueryProcessor.Operations
{
    internal class CreateTable
    {
        internal OperationStatus Execute() //Esta clase simplemente llama a CreateTable en el Store, que sería el encargado de crear una tabla en la base de datos
        {                                  //actualmente no se pasa nada a createtable como parametro, se debe agregar al execute que reciba datos para que al momento de crear la tabla se le pase la informacion necesaria.
            return Store.GetInstance().CreateTable(); //como por ejemplo el nombre de la tabla, la cantidad de columnas, etc.
        }
    }
}

