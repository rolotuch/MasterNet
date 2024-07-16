namespace MasterNet.Aplicacion.Photos
{
    public class PhotoUploadResult
    {
        //estos son los datos que nos devuleve el servicio cloud que contratemos con la referencia a las imagenes.
        public string? PublicId { get; set; }
        public string? Url { get; set; }
    }
}
