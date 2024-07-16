namespace MasterNet.Aplicacion.Cursos.CursoUpdate
{
    //clase que representa los datos que me enia el cliente
    public class CursoUpdateRequest
    {
        public string? Titulo { get; set; }
        public string? Descripcion { get; set; }
        public DateTime? FechaPublicacion { get; set; }

    }
}
