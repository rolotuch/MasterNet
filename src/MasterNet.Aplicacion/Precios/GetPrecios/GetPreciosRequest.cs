using MasterNet.Aplicacion.Core;

namespace MasterNet.Aplicacion.Precios.GetPrecios
{
    public class GetPreciosRequest : PagingParams
    {
        public string? Nombre { get; set; }        
    }
}
