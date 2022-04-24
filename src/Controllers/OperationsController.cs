using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CoreCodeCamp.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class OperationsController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public OperationsController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpOptions("ConnectionString")]
        public ActionResult<string> Get()
        {
            return configuration.GetConnectionString("CodeCamp");
        }

    }
}
