namespace MasterNet.Aplicacion.Core.ResponseResult
{
    public class ResponseResult<T>  //lo ponemos genericos porque puede utilizarse para cualquier cosa, por ejemplo un nuevo curos, nun login, un nuevo instructor etc
    {
        public bool IsSuccess { get; set; }
        public T? Value { get; set; }
        public string? Error { get; set; }
        //si quiero que estos elementos ya esten creados al momento de cargase la apliacion puedo realizar esto.

        public static ResponseResult<T> Success(T value) => new ResponseResult<T>
        {
            IsSuccess = true,
            Value = value
        };

        public static ResponseResult<T> Failure(string error) => new ResponseResult<T>
        {
            IsSuccess = false, // no se completo el resultado
            //Value = value //por consiguiente no vevuleve nada entonces mejor devolmemos el mensaje de error
            Error = error

        };
    }
}
