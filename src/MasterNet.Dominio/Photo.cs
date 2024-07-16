namespace MasterNet.Dominio
{
    public class Photo : BaseEntity
    {
        public string? Url { get; set; }
        public Guid? CursoId { get; set; }
        public Curso? Curso { get; set; }
        public string? PublicId { get; set; }
    }
}
