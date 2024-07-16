namespace MasterNet.Dominio
{
    public class CursoInstructor
    {
        public Guid? CursoId { get; set; } //clave foranea
        public Curso? Curso { get; set; } //referencia al objeto que lo representa

        public Guid? InstructorId { get; set; } //lllave foranea
        public Instructor? Instructor { get; set; } //referencia al objeto que lo representa
    }
}
