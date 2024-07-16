namespace MasterNet.Dominio
{
    public class Calificacion : BaseEntity
    {
        public string? Alumno { get; set; }
        public int Puntaje { get; set; }
        public string? Comentario { get; set; }
        public Guid? CursoId { get; set; } //relacion de una clave primaria con una clave foranea
        public Curso? Curso { get; set; } //aca lo instanciamos con la representacion del objeto, si se selecciona una foranea se debe de declarar su representacion de objeto

    }
}
