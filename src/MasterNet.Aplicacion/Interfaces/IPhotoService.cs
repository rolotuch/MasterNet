using MasterNet.Aplicacion.Photos;
using Microsoft.AspNetCore.Http;

namespace MasterNet.Aplicacion.Interfaces
{
    public interface IPhotoService
    {
        //agregamos dos metodos, mas adelante implementoamos la logica, despues de esto debemos inyectar el iphotoservice al interior de cursos
        Task<PhotoUploadResult> AddPhoto(IFormFile file);
        Task<string> DeletePhoto(string publicId);
    }
}
