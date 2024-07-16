namespace MasterNet.Dominio
{
    public class Instructor: BaseEntity
    {
        public string? Apellidos { get; set; }
        public string? Nombre { get; set; }
        public string? Grado { get; set; }

        public ICollection<Curso>? Cursos { get; set; } //instructor tambien tiene una coleccion de cursos
        public ICollection<CursoInstructor>? CursoInstructores { get; set; }  //instructor tambien tiene una coleccion de cursosinstructor
    }
}
