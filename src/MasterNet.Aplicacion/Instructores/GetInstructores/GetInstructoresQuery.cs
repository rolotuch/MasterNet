using AutoMapper.QueryableExtensions;
using AutoMapper;
using MasterNet.Aplicacion.Core;
using MasterNet.Aplicacion.Core.ResponseResult;
using MasterNet.Aplicacion.Instructores.GetInstructores;
using MasterNet.Dominio;
using MasterNet.Persistencia;
using MediatR;
using System.Linq.Expressions;

namespace MasterNet.Aplicacion.Instructores.GetInstructoresQuery;

public class GetInstructoresQuery
{

    //objeto de representacion de los querys
    public record GetInstructoresQueryRequest : IRequest<ResponseResult<PagedList<InstructorResponse>>>
    {
        public GetInstructoresRequest? InstructorRequest { get; set; }
    }

    //objeto de representacion de los queryhandler
    internal class GetInstructoresQueryHandler : IRequestHandler<GetInstructoresQueryRequest, ResponseResult<PagedList<InstructorResponse>>>
    {
        //declaracion de objetos para dbcontext y para mapper
        private readonly MasterNetDbContext _context;
        private readonly IMapper _mapper;
        //constructor de los objetos 
        public GetInstructoresQueryHandler(MasterNetDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        //logica para validacion de la paginacion
        public async Task<ResponseResult<PagedList<InstructorResponse>>> Handle(GetInstructoresQueryRequest request, CancellationToken cancellationToken)
        {

            IQueryable<Instructor> queryable = _context.Instructores!;
            //condiciones logicas el predicado
            var predicate = ExpressionBuilder.New<Instructor>();
            if (!string.IsNullOrEmpty(request.InstructorRequest!.Nombre))
            {
                predicate = predicate
                .And(y => y.Nombre!.Contains(request.InstructorRequest!.Nombre));
            }

            if (!string.IsNullOrEmpty(request.InstructorRequest!.Apellido))
            {
                predicate = predicate
                .And(y => y.Apellidos!.Contains(request.InstructorRequest!.Apellido));
            }

            if (!string.IsNullOrEmpty(request.InstructorRequest.OrderBy))
            {
                Expression<Func<Instructor, object>>? orderBySelector =
                request.InstructorRequest.OrderBy.ToLower() switch
                {
                    "nombre" => instructor => instructor.Nombre!,
                    "apellido" => instructor => instructor.Apellidos!,
                    _ => instructor => instructor.Nombre!
                };

                bool orderBy = request.InstructorRequest.OrderAsc.HasValue
                            ? request.InstructorRequest.OrderAsc.Value
                            : true;

                queryable = orderBy
                            ? queryable.OrderBy(orderBySelector)
                            : queryable.OrderByDescending(orderBySelector);
            }

            queryable = queryable.Where(predicate); //agregamos el predicate al queryable
            // se proyecta para que sea un tipo instructor response, es el tipo de datos que quiero devolver.
            var instructoresQuery = queryable
                        .ProjectTo<InstructorResponse>(_mapper.ConfigurationProvider)
                        .AsQueryable();
            //obtener la paginacion del objeto paginacion.
            var pagination = await PagedList<InstructorResponse>
                .CreateAsync(instructoresQuery,
                request.InstructorRequest.PageNumber,
                request.InstructorRequest.PageSize
                );

            return ResponseResult<PagedList<InstructorResponse>>.Success(pagination);
        }
    }


}


// datos que devolvemos al cliente
public record InstructorResponse(
    Guid? Id,
    string? Nombre,
    string? Apellido,
    string? Grado
)

{
    public InstructorResponse() : this(null, null, null, null)
    {
    }
}
