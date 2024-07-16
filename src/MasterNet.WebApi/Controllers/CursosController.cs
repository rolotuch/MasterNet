using MasterNet.Aplicacion.Core.ResponseResult;
using MasterNet.Aplicacion.Cursos.CursoCreate;
using MasterNet.Aplicacion.Cursos.CursoUpdate;
using MasterNet.Aplicacion.Cursos.GetCursos;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static MasterNet.Aplicacion.Cursos.CursoCreate.CursoCreateCommand;
using static MasterNet.Aplicacion.Cursos.CursoDelete.CursoDeleteCommand;
using static MasterNet.Aplicacion.Cursos.CursoReporteExcel.CursoReporteExcelQuery;
using static MasterNet.Aplicacion.Cursos.CursoUpdate.CursoUpdateCommand;
using static MasterNet.Aplicacion.Cursos.GetCurso.GetCursoQuery;
using static MasterNet.Aplicacion.Cursos.GetCursos.GetCursosQuery;

namespace MasterNet.WebApi.Controllers
{
    [ApiController]
    [Route("api/cursos")]
    public class CursosController : ControllerBase
    {
        private readonly ISender _sender;
        public CursosController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<ActionResult> PaginaciónCurso([FromQuery] GetCursosRequest request, CancellationToken cancellationToken)
        {
            //creamos el objeto query
            var query = new GetCursosQueryRequest { CursosRequest = request };
            var resultado = await _sender.Send(query, cancellationToken);

            return resultado.IsSuccess ? Ok(resultado.Value) : NotFound();

        }


        [HttpPost] //modificamos para que solo se llame a httppost
        //modificamos este metodo para tulizar el middleware
        public async Task<ActionResult<ResponseResult<Guid>>> CursoCreate([FromForm] CursoCreateRequest request, CancellationToken cancellationToken)
        {

            //throw new Exception("esta excepcion es forzada"); //para prueba del middleware.
            var command = new CursoCreateCommandRequest(request);
            //var resultado = await _sender.Send(command, cancellationToken);
            return await _sender.Send(command, cancellationToken);
            //modificamos para utilizar el middleware
            //return Ok(resultado);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseResult<Guid>>> CursoUpdate([FromBody] CursoUpdateRequest request, Guid id, CancellationToken cancellationToken)
        {
            var command = new CursoUpdateCommandRequest(request, id);
            var resultado = await _sender.Send(command, cancellationToken);
            return resultado.IsSuccess ? Ok(resultado?.Value) : BadRequest();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Unit>> CursoDelete(Guid id, CancellationToken cancellationToken)
        {
            var command = new CursoDeleteCommandRequest(id);
            var resultado = await _sender.Send(command, cancellationToken);

            return resultado.IsSuccess ? Ok(resultado?.Value) : BadRequest();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> CursoGet(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetCursoQueryRequest { Id = id };
            var resultado = await _sender.Send(query, cancellationToken);
            return resultado.IsSuccess ? Ok(resultado.Value) : BadRequest();
        }

        [HttpGet("rptexcel")]
        public async Task<IActionResult> ReporteCsv(CancellationToken cancellationToken)
        {
            var query = new CursoReporteExcelQueryRequest();
            var resultado = await _sender.Send(query, cancellationToken);

            //convertir la cadena en un arreglo de bytes a excel bytes
            byte[] excelBytes = resultado.ToArray();

            return File(excelBytes, "text/csv", "cursos.csv");
        }
    }
}
