using MasterNet.Aplicacion.Core;
using System.Net;
using System.Text.Json;

namespace MasterNet.WebApi.Middleware
{
    public class ExcepcionMiddleware
    {
        private readonly RequestDelegate _next; // este el que continua con el ciclo de vida del request (es un hilo de ejecucion)
        private readonly ILogger<ExcepcionMiddleware> _logger; // el ExcepcionMiddleware es esta misma clase 
        private readonly IHostEnvironment _env; // captura el ambiente donde se esta ejecutando la apliacion (test, desarrollo, producccion etc.

        //constructor
        public ExcepcionMiddleware(
            RequestDelegate next,
            ILogger<ExcepcionMiddleware> logger,
            IHostEnvironment env
        )
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        // este es el elemento mas importatne del middleware
        //este es el que al momento de ocurrir un evento dispara una ecepcion
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); //llamamos al delagate pasandole como parametro context para que cuando suceda algo en el context se atrape en el catch
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message); // captura el error
                context.Response.ContentType = "application/json";  // el formato de respuesta al cliente
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; // la data que devulevo, en este caso el statuscode 
                // modificamos el response; si estamos en ambiente de desarrollo enviamos el error con mas detalle, peros si estamos en produccion enviamos un mensaje generico
                var response = _env.IsDevelopment()
                                ? new AppExcepciones(
                                    context.Response.StatusCode,
                                    ex.Message,
                                    ex.StackTrace?.ToString() //este me muestra todo el codigo de error en formato string
                                    )
                                : new AppExcepciones( //esto para ambientes de producccion
                                    context.Response.StatusCode,
                                    "Internal Server Error" // simplemente un mensaje que diga internal server error.
                                );
                // esto va a viajar en tipo json, entonces debemos de serializarlo.
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var json = JsonSerializer.Serialize(response, options); //llamamos al metodo para serializar y le pasamos el objeto que queremos serializar que seria el response y co las opciones de serializacion el options

                await context.Response.WriteAsync(json); //ya serializado lo devolvemos al cliente.

            }
        }
    }
}
