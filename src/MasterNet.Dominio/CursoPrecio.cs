namespace MasterNet.Dominio
{
    public class CursoPrecio
    {
        public Guid? CursoId { get; set; } //clave foranea para curso (llaves compuestas)
        public Curso? Curso { get; set; } //como es una clave foranea debemos de colocar su representacion de objeto
        public Guid? PrecioId { get; set; } //clave foranea para precio (llaves compuestas)
        public Precio? Precio { get; set; } //como es una clave foranea debemos de colocar su representacion de objeto
    }
}
