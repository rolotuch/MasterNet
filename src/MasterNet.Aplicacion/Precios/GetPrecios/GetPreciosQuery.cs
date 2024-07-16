using AutoMapper;
using AutoMapper.QueryableExtensions;
using MasterNet.Aplicacion.Core;
using MasterNet.Aplicacion.Core.ResponseResult;
using MasterNet.Dominio;
using MasterNet.Persistencia;
using MediatR;
using System.Linq.Expressions;

namespace MasterNet.Aplicacion.Precios.GetPrecios;

public class GetPreciosQuery
{
    //implementacion de los objetos query
    public record GetPreciosQueryRequest : IRequest<ResponseResult<PagedList<PrecioResponse>>>
    {
        public GetPreciosRequest? PreciosRequest { get; set; }
    }

    //implementacion para los objetos queryhandler

    internal class GetPreciosQueryRequestHandler : IRequestHandler<GetPreciosQueryRequest, ResponseResult<PagedList<PrecioResponse>>>
    {
        //generamos los parametros para el context y mapper
        private readonly MasterNetDbContext _context;
        private readonly IMapper _mapper;

        //generamos el constructor
        public GetPreciosQueryRequestHandler(MasterNetDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        //implementamos la interfaz
        public async Task<ResponseResult<PagedList<PrecioResponse>>> Handle(GetPreciosQueryRequest request, CancellationToken cancellationToken)
        {
            //generamos el querable para precios
            IQueryable<Precio> queryable = _context.Precios!;


            //implementamos el predicado detipo expression builder de la tabla precio
            var predicate = ExpressionBuilder.New<Precio>();
            //implementamos la logica para la busqueda y el order
            if (!string.IsNullOrEmpty(request.PreciosRequest!.Nombre)) {
                predicate = predicate
                    .And(y => y.Nombre!.Contains(request.PreciosRequest!.Nombre));
            }

            //implementamos la logica para el order
            if (!string.IsNullOrEmpty(request.PreciosRequest!.OrderBy)) {
                //la espresion function
                Expression<Func<Precio, object>>? orderSelector = request.PreciosRequest.OrderBy.ToLower() switch
                {
                    "nombre" => x => x.Nombre!,
                    "precio" => x => x.PrecioActual!,
                    _ => x => x.Nombre!
                };

                bool orderBy = request.PreciosRequest.OrderAsc.HasValue ? request.PreciosRequest.OrderAsc.Value : true;

                queryable = orderBy ? queryable.OrderBy(orderSelector) : queryable.OrderByDescending(orderSelector);
            }

            queryable = queryable.Where(predicate);

            var preciosQuery = queryable.ProjectTo<PrecioResponse>(_mapper.ConfigurationProvider).AsQueryable();

            var pagination = await PagedList<PrecioResponse>.CreateAsync(preciosQuery, request.PreciosRequest.PageNumber, request.PreciosRequest.PageSize);

            return ResponseResult<PagedList<PrecioResponse>>.Success(pagination);
        }
    }

}


public record PrecioResponse(
    Guid? Id,
    string? Nombre,
    decimal? PrecioActual,
    decimal? PrecioPromocion
)

{
    public PrecioResponse() : this(null, null, null, null)
    {
    }
}
