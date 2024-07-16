using Microsoft.AspNetCore.Http;

namespace MasterNet.Aplicacion.Cursos.CursoCreate
{
    public class CursoCreateRequest
    {
        //clase request para crear un curso
        public string? Titulo { get; set; }
        public string? Descripcion { get; set; }
        public DateTime? FechaPublicacion { get; set; }
        public IFormFile? Foto { get; set; }
        public Guid? InstructorId { get; set; }
        public Guid? PrecioId { get; set; }
    }
}
