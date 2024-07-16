using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using MasterNet.Aplicacion.Interfaces;
using MasterNet.Aplicacion.Photos;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Masternet.Infraestructura.Photos
{
    public class PhotoService : IPhotoService //al heredar de la interfaz que creamos en aplicacion nos permite implementar la interfaz al realizarlo tenemos los metodos que creamos, entonces estamo listo para aplicar la logica
    {
        //primero necesitamos settear un objeto que represente al cloudinary
        private readonly Cloudinary _cloudinary; //en este punto no tenemos instalados ninguna libreria que nos de soporte para cludinary para .net; este se instala en infraestrucrua CludinaryDotnet 
        //Creamos un constructor para inyectarlo, pero este constructor sera de manera interna
        public PhotoService(IOptions<CloudinarySettings> config)
        {
            var account = new Account(config.Value.CloudName, config.Value.ApiKey, config.Value.ApiSecret); //con esto inicializamos el objeto con esos tres parametros
            // inicializamos el objeto
            _cloudinary = new Cloudinary(account); //con esto queda instanciado el cloudinary a nivel de este photoservice
        }
        public async Task<PhotoUploadResult> AddPhoto(IFormFile file)
        {
            //logica para agrear una photo a un curso.
            if (file.Length > 0)
            {
                await using var stream = file.OpenReadStream();
                var uploadparams = new ImageUploadParams //este es un libreria propia de cloudinary
                {
                    File = new FileDescription(file.FileName, stream), //este nos pide primero el archivo
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill") // tambien la transformacion
                };
                //para subirlo necesitamos llamar al cloudinary
                var uploadResult = await _cloudinary.UploadAsync(uploadparams);
                if (uploadResult.Error is not null)
                {
                    throw new Exception(uploadResult.Error.Message);
                }
                //si todo sale bien nos debe devover el id y la url para poder acceder a ella.
                return new PhotoUploadResult
                {
                    PublicId = uploadResult.PublicId,
                    Url = uploadResult.SecureUrl.ToString(),
                };

            }
            return null!;
        }

        public async Task<string> DeletePhoto(string publicId)
        {
            var deleteParams = new DeletionParams(publicId); // este DeletionParams es propio de cloudinary
            var deleteResult = await _cloudinary.DestroyAsync(deleteParams);

            return deleteResult.Result == "ok" ? deleteResult.Result! : null!;
        }
    }
}
