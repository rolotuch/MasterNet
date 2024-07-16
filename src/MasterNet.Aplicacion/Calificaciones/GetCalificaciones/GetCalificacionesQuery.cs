using AutoMapper;
using AutoMapper.QueryableExtensions;
using MasterNet.Aplicacion.Core;
using MasterNet.Aplicacion.Core.ResponseResult;
using MasterNet.Dominio;
using MasterNet.Persistencia;
using MediatR;
using System.Linq.Expressions;

namespace MasterNet.Aplicacion.Calificaciones.GetCalificaciones;

public class GetCalificacionesQuery
{

    public record GetCalificacionesQueryRequest : IRequest<ResponseResult<PagedList<CalificacionResponse>>>
    {
        public GetCalificacionesRequest? CalificacionesRequest { get; set; }
    }

    internal class GetCalificacionesQueryHandler : IRequestHandler<GetCalificacionesQueryRequest, ResponseResult<PagedList<CalificacionResponse>>>
    {
        private readonly MasterNetDbContext _context;
        private readonly IMapper _mapper;

        public GetCalificacionesQueryHandler(MasterNetDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ResponseResult<PagedList<CalificacionResponse>>> Handle(GetCalificacionesQueryRequest request, CancellationToken cancellationToken)
        {
            IQueryable<Calificacion> queryable = _context.Calificaciones!; //.Include(x => x.Curso);

            var predicate = ExpressionBuilder.New<Calificacion>();

            //filtro por alumno
            if (!string.IsNullOrEmpty(request.CalificacionesRequest!.Alumno))
            {
                predicate = predicate
                    .And(y => y.Alumno!.Contains(request.CalificacionesRequest!.Alumno));

            }
            if (request.CalificacionesRequest.CursoId is not null)
            {
                predicate = predicate
                .And(y => y.CursoId == request.CalificacionesRequest.CursoId);
            }

            //logica para el order by
            if (!string.IsNullOrEmpty(request.CalificacionesRequest!.OrderBy))
            {
                Expression<Func<Calificacion, object>> orderBySelector = request.CalificacionesRequest.OrderBy.ToLower() switch
                {
                    "alumno" => calificacion => calificacion.Alumno!, // cuando sea descripcion
                    "curso" => x => x.CursoId!,
                    _ => calificacion => calificacion.Alumno!
                };

                bool orderBy = request.CalificacionesRequest.OrderAsc.HasValue ? request.CalificacionesRequest.OrderAsc.Value : true;
                queryable = orderBy ? queryable.OrderBy(orderBySelector) : queryable.OrderByDescending(orderBySelector);
            }

            queryable = queryable.Where(predicate);

            var calificacionesQuery = queryable.ProjectTo<CalificacionResponse>(_mapper.ConfigurationProvider).AsQueryable();

            var pagination = await PagedList<CalificacionResponse>.CreateAsync(calificacionesQuery, request.CalificacionesRequest.PageNumber, request.CalificacionesRequest.PageSize);

            return ResponseResult<PagedList<CalificacionResponse>>.Success(pagination);
        }
    }

}


public record CalificacionResponse(
    string? Alumno,
    int? Puntaje,
    string? Comentario,
    string? NombreCurso
 )

{
    public CalificacionResponse() : this(null, null, null, null)
    {
    }
}