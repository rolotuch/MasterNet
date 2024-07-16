using MasterNet.Aplicacion.Core;

namespace MasterNet.Aplicacion.Calificaciones.GetCalificaciones
{
    public class GetCalificacionesRequest:PagingParams
    {
        public string? Alumno { get; set; }
        public Guid? CursoId { get; set; }
    }
}
