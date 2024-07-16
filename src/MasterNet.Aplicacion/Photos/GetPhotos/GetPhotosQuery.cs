namespace MasterNet.Aplicacion.Photos.GetPhotos;

public record PhotoResponse(
    Guid? Id,
    string? Url,
    Guid? CursoId
)

{
    public PhotoResponse() : this(null, null, null)
    {
    }
}