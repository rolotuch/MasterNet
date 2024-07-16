using MasterNet.Aplicacion.Core;

namespace MasterNet.Aplicacion.Instructores.GetInstructores
{
    public class GetInstructoresRequest: PagingParams
    {
        public string? Apellido { get; set; }
        public string? Nombre { get; set; }
    }
}
