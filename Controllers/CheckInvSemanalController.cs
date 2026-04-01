using API_PEDIDOS.ModelsDB2;
using API_PEDIDOS.ModelsDBP;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Xml.Linq;

namespace API_PEDIDOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckInvSemanalController : ControllerBase
    {
        private readonly ILogger<CatalogosController> _logger;
        protected BD2Context _contextdb2;
        protected DBPContext _dbpContext;

        public CheckInvSemanalController(ILogger<CatalogosController> logger, BD2Context db2c, DBPContext dbpc)
        {
            _logger = logger;
            _contextdb2 = db2c;
            _dbpContext = dbpc;
        }

        [HttpGet]
        [Route("geArticulos")]
        public async Task<ActionResult> GetArticulos()
        {
            try
            {
                var query = _contextdb2.Articulos1
                    .GroupJoin(
                        _contextdb2.Articuloscamposlibres,
                        art => art.Codarticulo,
                        artcl => artcl.Codarticulo,
                        (art, artclGroup) => new { art, artclGroup })
                    .SelectMany(
                        x => x.artclGroup.DefaultIfEmpty(),
                        (x, artcl) => new { x.art, artcl })
                    .Where(x => x.art.Descatalogado == "F"
                                && !x.art.Descripcion.StartsWith("*")
                                && x.artcl != null && x.artcl.RegularizaSemanal != "T")
                    .Select(x => new { x.art.Codarticulo, x.art.Descripcion });

                //var articulos = _contextdb2.Articulos1.Where(x => x.Descatalogado == "F" && !x.Descripcion.StartsWith("*")).ToList();
                var articulos = query.ToList(); 
                List<object> data = new List<object>();
                foreach (var articulo in articulos) 
                {
                    data.Add(new { cod = articulo.Codarticulo, descripcion = articulo.Descripcion, marca = ""}); 
                }

                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                return StatusCode(500, new
                {
                    Success = false,
                    Message = ex.ToString(),
                });
            }
        }

        [HttpPost]
        [Route("agregarArticulos")]
        public async Task<ActionResult> agregarArticulos([FromForm] string jdata)
        {
            try
            {
                int[] articulos = System.Text.Json.JsonSerializer.Deserialize<int[]>(jdata);

                foreach (int art in articulos) 
                {
                    var artbd = _dbpContext.CheckInvSemanals.Where(x => x.Codarticulo == art).FirstOrDefault();
                    if (artbd == null) 
                    {
                        _dbpContext.CheckInvSemanals.Add(new CheckInvSemanal() { Codarticulo = art });
                        await _dbpContext.SaveChangesAsync();
                    }
                   
                }
             
                return Ok(articulos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                return StatusCode(500, new
                {
                    Success = false,
                    Message = ex.ToString(),
                });
            }
        }

        [HttpPost]
        [Route("eliminarArticulos")]
        public async Task<ActionResult> eliminarArticulos([FromForm] string jdata)
        {
            try
            {
                int[] articulos = System.Text.Json.JsonSerializer.Deserialize<int[]>(jdata);

                foreach (int art in articulos)
                {
                    var artdb = _dbpContext.CheckInvSemanals.Where(x => x.Codarticulo == art).FirstOrDefault();
                    if (artdb != null) 
                    {
                        _dbpContext.CheckInvSemanals.Remove(artdb); 
                    }
                }

                await _dbpContext.SaveChangesAsync();
                return Ok(articulos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                return StatusCode(500, new
                {
                    Success = false,
                    Message = ex.ToString(),
                });
            }
        }

        [HttpGet]
        [Route("geArticulosbd")]
        public async Task<ActionResult> GetArticulosBD()
        {
            try
            {
                List<Object> dataart = new List<Object>();
               var data = _dbpContext.CheckInvSemanals.ToList();
                foreach (var item in data)
                {
                    var art = _contextdb2.Articulos1.Where(x => x.Codarticulo == item.Codarticulo).FirstOrDefault();
                    var marca = _contextdb2.Marcas.Where(x => x.Codmarca == art.Marca).FirstOrDefault();
                    string nomseccion = "";
                    if(marca != null) { nomseccion = marca.Descripcion; }
                    if (art != null) 
                    {
                        dataart.Add(new { cod = art.Codarticulo, descripcion = art.Descripcion, marca = nomseccion, referencia = art.Refproveedor, prioridad = item.Prioridad, umedida = art.Unidadmedida });
                    }
                }

                return Ok(dataart); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                return StatusCode(500, new
                {
                    Success = false,
                    Message = ex.ToString(),
                });
            }
        }

        [HttpPost]
        [Route("actualizarArticulos")]
        public async Task<ActionResult> actualizarArticulos([FromForm] string jdata)
        {
            try
            {
                List<ArtBDModel> articulos = System.Text.Json.JsonSerializer.Deserialize<List<ArtBDModel>>(jdata);

                foreach (ArtBDModel art in articulos)
                {
                    var artdb = _dbpContext.CheckInvSemanals.Where(x => x.Codarticulo == art.cod).FirstOrDefault();
                    if (artdb != null)
                    {
                        artdb.Prioridad = art.prioridad; 
                        _dbpContext.CheckInvSemanals.Update(artdb);
                        await _dbpContext.SaveChangesAsync();
                    }
                }
                return Ok(articulos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                return StatusCode(500, new
                {
                    Success = false,
                    Message = ex.ToString(),
                });
            }
        }

    }

    public class ArtBDModel 
    {
        public int cod {  get; set; }
        public int prioridad { get; set; }
    }
}
