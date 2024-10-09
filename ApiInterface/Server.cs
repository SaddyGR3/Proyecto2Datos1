using System.Net.Sockets;
using System.Net;
using System.Text;
using ApiInterface.InternalModels;
using System.Text.Json;
using ApiInterface.Exceptions;
using ApiInterface.Processors;
using ApiInterface.Models;
using QueryProcessor.Exceptions;
using QueryProcessor;
using Entities;

namespace ApiInterface
{
    public class Server
    {
        private static IPEndPoint serverEndPoint = new(IPAddress.Loopback, 11000);
        private static int supportedParallelConnections = 1;

        public static async Task Start()
        {
            using Socket listener = new(serverEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(serverEndPoint);
            listener.Listen(supportedParallelConnections);
            Console.WriteLine($"Server ready at {serverEndPoint}");

            while (true)
            {
                var handler = await listener.AcceptAsync();
                try
                {
                    var rawMessage = GetMessage(handler);
                    var requestObject = ConvertToRequestObject(rawMessage);
                    var response = ProcessRequest(requestObject);
                    SendResponse(response, handler);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    await SendErrorResponse("Unknown exception", handler);
                }
                finally
                {
                    handler.Close();
                }
            }
        }

        private static string GetMessage(Socket handler)
        {
            using (NetworkStream stream = new NetworkStream(handler))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadLine() ?? string.Empty;
            }
        }

        private static Request ConvertToRequestObject(string rawMessage)
        {
            try
            {
                // Deserialize the raw JSON message into a Request object
                return JsonSerializer.Deserialize<Request>(rawMessage) ?? throw new InvalidRequestException();
            }
            catch (JsonException)
            {
                throw new InvalidRequestException();
            }
        }
        private static Response ProcessRequest(Request requestObject)
        {
            try
            {
                var sqlSentence = requestObject.RequestBody;
                var (status, resultList) = SQLQueryProcessor.Execute(sqlSentence);

                return new Response
                {
                    Status = status,
                    Request = requestObject,  // Inicializamos correctamente 'Request'
                    ResponseBody = status == OperationStatus.Success
                        ? (resultList != null ? string.Join(", ", resultList) : "Operation successful")
                        : "Operation failed"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing request: {ex.Message}");
                return new Response
                {
                    Status = OperationStatus.Error,
                    Request = requestObject,  // Asegúrate de pasar el objeto 'requestObject', aunque sea el que falló
                    ResponseBody = "Error processing the request."
                };
            }
        }


        private static void SendResponse(Response response, Socket handler)
        {
            using (NetworkStream stream = new NetworkStream(handler))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                // Serializamos el objeto Response a JSON y lo enviamos
                writer.WriteLine(JsonSerializer.Serialize(response));
                writer.Flush();  // Asegurarse de que todo el contenido se envíe
            }
        }


        private static async Task SendErrorResponse(string reason, Socket handler)
        {
            var errorResponse = new Response
            {
                Status = OperationStatus.Error,
                Request = null!,  // No hay solicitud válida en este caso
                ResponseBody = reason,
                Data = null
            };

            using (NetworkStream stream = new NetworkStream(handler))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                await writer.WriteLineAsync(JsonSerializer.Serialize(errorResponse));
                await writer.FlushAsync();
            }
        }
    }
}
