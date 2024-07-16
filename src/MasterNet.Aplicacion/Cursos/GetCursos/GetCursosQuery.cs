using AutoMapper;
using AutoMapper.QueryableExtensions;
using MasterNet.Aplicacion.Core;
using MasterNet.Aplicacion.Core.ResponseResult;
using MasterNet.Aplicacion.Cursos.GetCurso;
using MasterNet.Dominio;
using MasterNet.Persistencia;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MasterNet.Aplicacion.Cursos.GetCursos
{
    public class GetCursosQuery
    {
        //aca ira la clase que los objetos que representa los query

        public record GetCursosQueryRequest : IRequest<ResponseResult<PagedList<CursoResponse>>>
        {
            public GetCursosRequest? CursosRequest { get; set; }
        }
        //aca ira la clase que los objetos que representa los queryhandlers
        internal class GetCursosQueryHandler : IRequestHandler<GetCursosQueryRequest, ResponseResult<PagedList<CursoResponse>>>
        {
            ///implementamos la para ver lo que necesitamos tener para la paginacion
            private readonly MasterNetDbContext _context;
            private readonly IMapper _mapper;

            public GetCursosQueryHandler(MasterNetDbContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            //implementamos la interfaz
            public async Task<ResponseResult<PagedList<CursoResponse>>> Handle(GetCursosQueryRequest request, CancellationToken cancellationToken)
            {
                //implementamos la logica de la paginacion
                IQueryable<Curso> queryable = _context.Cursos!// con esto ya me devuelve una consulta; aca puedo mete un where _context.Cursos!.Where nombre ='xxx'
                                            .Include(x => x.Instructores) // aca le indico que me devuelva los resultado de instructores
                                            .Include(x => x.Calificaciones)
                                            .Include(x => x.Precios);
                //todo lo anterior es parte del objeto iquerable
                //ahora veremos como implementar otros parametros
                //inicimaos con el titulo
                var predicate = ExpressionBuilder.New<Curso>(); // creamos un predicado con la ExpressionBuilder lo creamos dentro de core.
                if (!string.IsNullOrEmpty(request.CursosRequest!.Titulo)) // siempre que el titulo no sea nulo entonces aplicale la condicion
                {
                    predicate = predicate // 
                    .And(y => y.Titulo!.ToLower() // que todo lo convierta a minusculas
                    .Contains(request.CursosRequest.Titulo.ToLower())); // si lo que envia esta contenido dentro del request lo muestre.
                }

                //logica para la descripcion
                if (!string.IsNullOrEmpty(request.CursosRequest!.Descripcion))
                {
                    predicate = predicate
                    .And(y => y.Descripcion!.ToLower()
                    .Contains(request.CursosRequest.Descripcion.ToLower()));
                }

                //logica para la ordenacion
                if (!string.IsNullOrEmpty(request.CursosRequest!.OrderBy)) //mientras order by no sea null
                {
                    Expression<Func<Curso, object>>? orderBySelector = //aca usamos un expression function asi es como se utiliza.
                                    request.CursosRequest.OrderBy!.ToLower() switch // estamos evaluando el order by a traves de un switch
                                    {
                                        "titulo" => curso => curso.Titulo!, //cuando sea titulo 
                                        "descripcion" => curso => curso.Descripcion!, // cuando sea descripcion
                                        _ => curso => curso.Titulo! // si no me envia un valor que conincida con el titulo o la descripcion lo ordena por el titulo
                                    };
                    // evaluamos si el orden es asc o desc
                    bool orderBy = request.CursosRequest.OrderAsc.HasValue //si el ordenamineto asc tiene algun valor
                                ? request.CursosRequest.OrderAsc.Value // setea ese valor
                                : true; // caso contrario sea true es decir sea ascendenete

                    //el anterio bool se puede resumir de esta forma bool orderByAsc = request.CursosRequest.OrderAsc ?? true; y hace exactamente lo mismo.
                    //
                    queryable = orderBy
                                ? queryable.OrderBy(orderBySelector) // si el order by es true ordena por el order by selector
                                : queryable.OrderByDescending(orderBySelector); // en caso contrario que sea desc sobre el orderby selector
                }

                queryable = queryable.Where(predicate); // agregamos el predicado al queryable este seria la segunda parte de la consulta a la bd
                // ahora psamos el rsultado de este queryable al formato de cursoresponse
                var cursosQuery = queryable
                .ProjectTo<CursoResponse>(_mapper.ConfigurationProvider)
                .AsQueryable();

                var pagination = await PagedList<CursoResponse>.CreateAsync(
                    cursosQuery,
                    request.CursosRequest.PageNumber,
                    request.CursosRequest.PageSize
                );

                return ResponseResult<PagedList<CursoResponse>>.Success(pagination); //devolvemos la paginacion.

            }
        }
    }
}