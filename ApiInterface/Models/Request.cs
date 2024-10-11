namespace ApiInterface.InternalModels
{
    internal enum RequestType
    {
        SQLSentence = 0,  
        Insert = 1,       // se puede agregar tipos específicos para cada operación SQL
        Delete = 2,
        Select = 3
    }

    internal class Request
    {
        // Diferencia el tipo de solicitud, por ejemplo, INSERT, DELETE, SELECT
        public required RequestType RequestType { get; set; }

        // El comando SQL en formato de texto que llega desde el cliente PowerShell
        public required string RequestBody { get; set; }
    }
}
