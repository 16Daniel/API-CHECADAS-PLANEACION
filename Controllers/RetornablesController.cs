using API_PEDIDOS.ModelsDB2;
using API_PEDIDOS.ModelsDBP;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_PEDIDOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RetornablesController : ControllerBase
    {

        private readonly ILogger<CatalogosController> _logger;
        protected BD2Context _contextdb2;
        protected DBPContext _dbpContext;

        public RetornablesController(ILogger<CatalogosController> logger, BD2Context db2c, DBPContext dbpc)
        {
            _logger = logger;
            _contextdb2 = db2c;
            _dbpContext = dbpc;
        }


        [HttpGet]
        [Route("getData")]
        public async Task<ActionResult> getData()
        {
            try
            {
                List<RetornableModel> data = new List<RetornableModel>();
               var datadb = _dbpContext.Retornables.ToList();

                foreach (var d in datadb) 
                {
                    var articulo = _contextdb2.Articulos1.Where(x=> x.Codarticulo == d.Codart).FirstOrDefault();
                    var proveedor = _contextdb2.Proveedores.Where(x => x.Codproveedor == d.Codprov).FirstOrDefault();
                    data.Add(new RetornableModel
                    {
                        id = d.Id,
                        rfc = proveedor.Nif20,
                        nomprov = proveedor.Nomproveedor,
                        articulo = articulo.Descripcion
                    });
                }

                return StatusCode(StatusCodes.Status200OK,data);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.ToString() });
            }


        }



        [HttpPost]
        [Route("AgregarArticulo")]
        public async Task<ActionResult> save([FromBody] Retornable model)
        {
            try
            {
                var reg = _dbpContext.Retornables.Where(x => x.Codprov == model.Codprov && x.Codart == model.Codart).FirstOrDefault();

                if (reg == null) 
                {

                    await _dbpContext.Retornables.AddAsync(model);
                    await _dbpContext.SaveChangesAsync();
                }

                return StatusCode(StatusCodes.Status200OK);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.ToString() });
            }


        }


        [HttpDelete]
        [Route("DeleteArt/{id}")]
        public async Task<ActionResult> delete(int id)
        {
            try
            {
                var reg = _dbpContext.Retornables.Where(x => x.Id == id).FirstOrDefault();
                if (reg != null) 
                {
                    _dbpContext.Retornables.Remove(reg);
                    await _dbpContext.SaveChangesAsync();
                }
                return StatusCode(StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.ToString() });
            }


        }

    }

    public class RetornableModel 
    {
        public int id { get; set; }
        public string rfc { get; set; }
        public string nomprov { get; set; }
        public string articulo { get; set; }
    }
}
