namespace ApiInterface.InternalModels
{
    internal enum RequestType
    {
        SQLSentence = 0,  // Puedes mantener esta opción si tienes solicitudes generales
        Insert = 1,       // Agregamos tipos específicos para cada operación SQL
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
