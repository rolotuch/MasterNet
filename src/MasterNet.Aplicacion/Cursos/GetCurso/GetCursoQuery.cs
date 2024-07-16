using AutoMapper;
using AutoMapper.QueryableExtensions;
using MasterNet.Aplicacion.Calificaciones.GetCalificaciones;
using MasterNet.Aplicacion.Core.ResponseResult;
using MasterNet.Aplicacion.Instructores.GetInstructoresQuery;
using MasterNet.Aplicacion.Photos.GetPhotos;
using MasterNet.Aplicacion.Precios.GetPrecios;
using MasterNet.Persistencia;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MasterNet.Aplicacion.Cursos.GetCurso;

public class GetCursoQuery
{
    //representa la definicio de los objetos query
    public record GetCursoQueryRequest : IRequest<ResponseResult<CursoResponse>>
    {
        public Guid Id { get; set; } // el id que nos envia el cliente para poder hacer la consulta
    }

    //ahora creamos el queryhandler
    internal class GetCursoQueryHandler : IRequestHandler<GetCursoQueryRequest, ResponseResult<CursoResponse>>
    {
        private readonly MasterNetDbContext _context;
        private readonly IMapper _mapper;

        public GetCursoQueryHandler(MasterNetDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ResponseResult<CursoResponse>> Handle(GetCursoQueryRequest request, CancellationToken cancellationToken)
        {
            var curso = await _context.Cursos!.Where(x => x.Id == request.Id)
                            .Include(x => x.Instructores)
                            .Include(x => x.Precios)
                            .Include(x => x.Calificaciones)
                            .Include(x => x.Photos)
                            .ProjectTo<CursoResponse>(_mapper.ConfigurationProvider)  //proyecte este resultado tipo a uno de curso reponse.
                            .FirstOrDefaultAsync(); //esto es para pasar solo un valor, el que nos envian del request

            return ResponseResult<CursoResponse>.Success(curso!);
        }
    }
}


//public record CursoResponse(
//    Guid? Id,
//    string? Titulo,
//    string? Descripcion,
//    string? Imagen,
//    DateTime? FechaPublicacion,
//    List<InstructorResponse>? Instructores,
//    List<CalificacionResponse>? Calificaciones,
//    List<PrecioResponse>? Precios,
//    List<PhotoResponse>? Photos
//);

//en mi caso no me funciono con la propuesta de arriba me solicitaba un constructor, asi que lo solucione de esta forma.
public record CursoResponse
{
    public Guid? Id { get; init; }
    public string? Titulo { get; init; }
    public string? Descripcion { get; init; }
    public string? Imagen { get; init; }
    public DateTime? FechaPublicacion { get; init; }
    public List<InstructorResponse>? Instructores { get; init; }
    public List<CalificacionResponse>? Calificaciones { get; init; }
    public List<PrecioResponse>? Precios { get; init; }
    public List<PhotoResponse>? Photos { get; init; }

    public CursoResponse() { }

    public CursoResponse(Guid? Id, string? Titulo, string? Descripcion, string? Imagen, DateTime? FechaPublicacion, List<InstructorResponse>? Instructores, List<CalificacionResponse>? Calificaciones, List<PrecioResponse>? Precios, List<PhotoResponse>? Photos)
    {
        this.Id = Id;
        this.Titulo = Titulo;
        this.Descripcion = Descripcion;
        this.Imagen = Imagen;
        this.FechaPublicacion = FechaPublicacion;
        this.Instructores = Instructores;
        this.Calificaciones = Calificaciones;
        this.Precios = Precios;
        this.Photos = Photos;
    }
}