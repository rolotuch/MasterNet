using FluentValidation;
using MasterNet.Aplicacion.Core.ResponseResult;
using MasterNet.Aplicacion.Interfaces;
using MasterNet.Dominio;
using MasterNet.Persistencia;
using MediatR;
using static MasterNet.Aplicacion.Cursos.CursoCreate.CursoCreateCommand;

namespace MasterNet.Aplicacion.Cursos.CursoCreate
{
    public class CursoCreateCommand
    {
        //command
        //aca podemos crear una clase o un record (un record te permite almancenar datos en memoria) la clase tiene mas funcionalidades que un record
        //a manera de ejemplo aca creamo un record

        //CursoCreateRequest este es la representacion que te envia el cliente desde el web api, este lo obtenemos desde el CursoCreateRequest
        //public record CursoCreateCommandRequest(CursoCreateRequest cursoCreateRequest) : IRequest<Guid>; //aca nos pide que devemos devover al momento de efecutarse la insersion, en este caso devolvemos el id, el objeto request es un parametro del objeto command
        public record CursoCreateCommandRequest(CursoCreateRequest cursoCreateRequest) : IRequest<ResponseResult<Guid>>; //modificamos el GUIOD para poder utilizar un midlleware que trabaje los mensajes, el responseResult
        //commandHandler
        internal class CursoCreateCommandHandler : IRequestHandler<CursoCreateCommandRequest, ResponseResult<Guid>> // recibe mensjae sdel command que declaramos arriba y le pasamos el id del curso
        {
            //creamos la instancia del dbcontext
            private readonly MasterNetDbContext _context;  //este representa la session del entity con la cual ya se puede tener acceso a la bd
            private readonly IPhotoService _photoService;  //estamos inyectando Iphoservice para poder utilizar cloudinary para agregar photos, pero aun no hemos configurado en el contenedor de dependencias en program cs

            //generamos el constructor, instanciamos el dbcontext para poder inyectarlo despues
            public CursoCreateCommandHandler(MasterNetDbContext context, IPhotoService photoService)
            {
                _context = context;
                _photoService = photoService;
            }

            //interfaz del metodo hadnler
            public async Task<ResponseResult<Guid>> Handle(
                CursoCreateCommandRequest request, //parametro que representa al objeto command como tal, el mensaje que esta llegando
                CancellationToken cancellationToken //cancelacion del token.
            )
            {
                var cursoId = Guid.NewGuid();
                var curso = new Curso
                {
                    Id = cursoId,
                    Titulo = request.cursoCreateRequest.Titulo,   // el parametro titulo lo tiene el request (CursoCreateCommandRequest request)
                    Descripcion = request.cursoCreateRequest.Descripcion,
                    FechaPublicacion = request.cursoCreateRequest.FechaPublicacion //FALTA LAS PHOTOS

                };

                //logica para agregar una foto
                //primero validamos si existe un foto
                if (request.cursoCreateRequest.Foto is not null)
                {
                    var photoUploadResul = await _photoService.AddPhoto(request.cursoCreateRequest.Foto);

                    //creamos una nueva foto
                    var photo = new Photo
                    {
                        Id = Guid.NewGuid(), // se agrego porque se creo un nuevo atribuo llamado publicId
                        Url = photoUploadResul.Url,
                        PublicId = photoUploadResul.PublicId, //aca da problema porque el id es de tipo guid y publicid es de tipo string para resolver en domiio photo agreamos un nuevo campo de tipo string llamado PublicId
                        CursoId = cursoId // si no le pongo esto no sabria a que curso le estoy asignando la foto, en ese momento se hace la relacion entgre foto y curso.
                    };
                    //arriba creamos un nuevo objeto de photo ahora lo que devemos realizar es agregar ese objeto a cursos, recordemos tambien que el objeto curso ya esta creado
                    curso.Photos = new List<Photo> { photo };
                }

                //agregamos condicion para agregar un instructor
                if (request.cursoCreateRequest.InstructorId is not null)
                {
                    //verificamos que si el id que me envian esta en la tabla de instructor
                    var instrutor = _context.Instructores!.FirstOrDefault(x => x.Id == request.cursoCreateRequest.InstructorId);
                    if (instrutor is null) // si no existe enviamos un mensaje indicando que el id no exsite
                    {
                        return ResponseResult<Guid>.Failure("No se encontro el Instructor");
                    }

                    //si existe agregamos el instructor al curso
                    curso.Instructores = new List<Instructor> { instrutor };

                }

                //agregamos un precio al cruso

                if (request.cursoCreateRequest.PrecioId is not null)
                {
                    //verificamos que si el id que me envian esta en la tabla de instructor
                    var precio = _context.Precios!.FirstOrDefault(x => x.Id == request.cursoCreateRequest.PrecioId);
                    if (precio is null) // si no existe enviamos un mensaje indicando que el id no exsite
                    {
                        return ResponseResult<Guid>.Failure("No se encontro el precio");
                    }

                    //si existe agregamos el instructor al curso
                    curso.Precios = new List<Precio> { precio };

                }

                //el objeto creado lo agregamos a la session del ef
                _context.Add(curso);
                //grabamos a la base de datos
                var resultado = await _context.SaveChangesAsync(cancellationToken) > 0;  //con cancellationtoken podemos monitorear la transaccion.


                // esto es para ver si hay datos que los inserte si no hay que no haga nada.
                return resultado
                    ? ResponseResult<Guid>.Success(curso.Id)
                    : ResponseResult<Guid>.Failure("No se pudo insertar el curso");
                //devolvmeos un guid en este caso curso.id
                //con el middlweare creamos una instancia de success
                //return  curso.Id;
                //return ResponseResult<Guid>.Success(curso.Id);
            }
        }
    }

    public class CursoCreateCommandRequestValidator : AbstractValidator<CursoCreateCommandRequest>
    {
        public CursoCreateCommandRequestValidator()
        {

            RuleFor(x => x.cursoCreateRequest).SetValidator(new CursoCreateValidator());
            RuleFor(x => x.cursoCreateRequest).SetValidator(new CursoCreateValidator());

        }
    }
}
