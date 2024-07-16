namespace MasterNet.Dominio
{
    public class Curso : BaseEntity
    {
        public string? Titulo { get; set; }
        public string? Descripcion { get; set; }
        public DateTime? FechaPublicacion { get; set; }
        public ICollection<Calificacion>? Calificaciones { get; set; } //curso tiene una coleccion de calificaciones por ello lo declaramos asi.

        public ICollection<Precio>? Precios { get; set; } //curso tambien tiene una coleccion de precios
        public ICollection<CursoPrecio>? CursoPrecios { get; set; } //un curso contiene una coleccion de cursoprecio (es de muchos a muchos)

        public ICollection<Instructor>? Instructores { get; set; } //curso tambien tiene una coleccion de instructores
        public ICollection<CursoInstructor>? CursoInstructores { get; set; } //un curso contiene una coleccion de cursoInstructor (es de muchos a muchos)= null;

        public ICollection<Photo>? Photos { get; set; } //un curso contiene una coleccion de fotos
    }
}
