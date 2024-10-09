using ApiInterface.InternalModels;
using Entities;

namespace ApiInterface.Models
{
    internal class Response
    {
        // Mantener la solicitud original, por si es útil en la respuesta
        public required Request Request { get; set; }

        // Indica si la operación fue exitosa o no
        public required OperationStatus Status { get; set; }

        // Cuerpo de la respuesta; podría ser un mensaje de éxito o error, o los resultados de una consulta
        public required string ResponseBody { get; set; }

        // Lista opcional para datos en caso de un SELECT, por ejemplo, los resultados
        public List<string>? Data { get; set; }
    }
}
