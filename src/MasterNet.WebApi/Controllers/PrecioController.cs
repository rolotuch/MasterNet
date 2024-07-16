using MasterNet.Aplicacion.Precios.GetPrecios;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static MasterNet.Aplicacion.Precios.GetPrecios.GetPreciosQuery;

namespace MasterNet.WebApi.Controllers
{
    [ApiController]
    [Route("api/precios")]
    public class PrecioController : ControllerBase
    {        
        private readonly ISender _sender;

        public PrecioController(ISender sender)
        {
            _sender = sender;
        }

        // GET: InstructoresController
        [HttpGet]
        public async Task<ActionResult> PaginationPrecio([FromQuery] GetPreciosRequest request, CancellationToken cancellationToken)
        {
            var query = new GetPreciosQueryRequest { PreciosRequest = request };

            var resultado = await _sender.Send(query, cancellationToken);

            return resultado.IsSuccess ? Ok(resultado.Value) : NotFound();
        }     
    }
}
