using MasterNet.Aplicacion.Calificaciones.GetCalificaciones;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static MasterNet.Aplicacion.Calificaciones.GetCalificaciones.GetCalificacionesQuery;

namespace MasterNet.WebApi.Controllers
{
    [ApiController]
    [Route("api/calificaciones")]
    public class CalificacionController : ControllerBase
    {
        private readonly ISender _sender;

        public CalificacionController(ISender sender)
        {
            _sender = sender;
        }
        
        [HttpGet]
        public async Task<ActionResult> PaginationCalificacion([FromQuery] GetCalificacionesRequest request, CancellationToken cancellationToken)
        {
            var query = new GetCalificacionesQueryRequest { CalificacionesRequest = request };
            var resultado = await _sender.Send(query, cancellationToken);
            return resultado.IsSuccess ? Ok(resultado.Value) : NotFound();
        }
    }
}
