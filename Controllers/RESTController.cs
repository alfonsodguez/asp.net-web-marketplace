using marketplace.Models;
using marketplace.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace marketplace.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class RESTController : ControllerBase
    {
        private readonly IDBAccess _accesoDB;

        public RESTController(IDBAccess servicioDBInyect)
        {
            this._accesoDB = servicioDBInyect;
        }


        [HttpGet]
        public async Task<IActionResult> DevolverMunicipios([FromQuery] int codpro)
        {
            List<Municipio> municipios = await this._accesoDB.DevolverMunicipios(codpro);
            return new JsonResult(municipios);
        }
    }
}
