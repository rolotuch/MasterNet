using Microsoft.AspNetCore.Mvc;

namespace MasterNet.WebApi.Controllers
{
    [ApiController]
    [Route("Demo")]
    public class DemoController : Controller
    {
        // GET: DemoController
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public DemoController(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }

        [HttpGet("getstring")]
        public string GetNombre()
        {
            return "vaxidrez.com";
        }

        [HttpGet("ambiente")]
        public IActionResult GetAmbiente()
        {
            var mensaje = _configuration.GetValue<string>("MiVariable");
            var ambiente = _environment.EnvironmentName;

            return Ok(new { Ambiente = ambiente, Mensaje = mensaje }); // el new {} se conoce como una funcion anonima ya que no depende de un objeto en especifico.
        }

    }
}