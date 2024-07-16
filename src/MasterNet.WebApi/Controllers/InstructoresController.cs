using MasterNet.Aplicacion.Instructores.GetInstructores;
using MediatR;
using Microsoft.AspNetCore.Mvc;

using static MasterNet.Aplicacion.Instructores.GetInstructoresQuery.GetInstructoresQuery;

namespace MasterNet.WebApi.Controllers
{
    [ApiController]
    [Route("api/instructores")]
    public class InstructoresController : ControllerBase
    {
        private readonly ISender _sender;

        public InstructoresController(ISender sender)
        {
            _sender = sender;
        }

        // GET: InstructoresController
        [HttpGet]
        public async Task<ActionResult> PaginationInstructor([FromQuery] GetInstructoresRequest request, CancellationToken cancellationToken)
        {
            var query = new GetInstructoresQueryRequest { InstructorRequest = request };

            var resultado = await _sender.Send(query, cancellationToken);

            return resultado.IsSuccess ? Ok(resultado.Value) : NotFound();
        }        
    }
}
