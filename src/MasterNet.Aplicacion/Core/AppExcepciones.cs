namespace MasterNet.Aplicacion.Core
{

    public class AppExcepciones
    {
        public AppExcepciones(int statusCode, string? message, string? details = null)
        {
            StatusCode = statusCode;
            Message = message;
            Details = details;
        }

        //valores posibles para la excepcion
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public string? Details { get; set; }
    }

}
