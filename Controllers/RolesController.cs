using API_PEDIDOS.ModelsDB2;
using API_PEDIDOS.ModelsDBP;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text.Json;

namespace API_PEDIDOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly ILogger<CatalogosController> _logger;
        protected BD2Context _contextdb2;
        protected DBPContext _dbpContext;


        public RolesController(ILogger<CatalogosController> logger, BD2Context db2c, DBPContext dbpc)
        {
            _logger = logger;
            _contextdb2 = db2c;
            _dbpContext = dbpc;
        }

        [HttpGet]
        [Route("getRoles")]
        public async Task<ActionResult> GetRoles()
        {
            try
            {
                List<CatRole> roles = new List<CatRole>();
                roles = _dbpContext.CatRoles.ToList(); 
                return StatusCode(200, roles);
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
        [Route("getRutas")]
        public async Task<ActionResult> GetRutas()
        {
            try
            {
                List<CatRuta> rutas = new List<CatRuta>();
                rutas = _dbpContext.CatRutas.ToList();
                return StatusCode(200, rutas);
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
        [Route("getRutasRol/{idr}")]
        public async Task<ActionResult> GetRutasRol(int idr)
        {
            try
            {
                List<CatRuta> rutas = new List<CatRuta>();
                var idrutas = _dbpContext.AccesosRutas.Where(x => x.IdRol == idr).ToList();

                foreach (var item in idrutas) 
                {
                    var ruta = _dbpContext.CatRutas.Where(x => x.Id == item.IdRuta).FirstOrDefault();
                    if (ruta != null) 
                    {
                        rutas.Add(ruta);    
                    }   

                }

                return StatusCode(200, rutas);
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
        [Route("createRol")]
        public async Task<ActionResult> createrol(CatRole model)
        {
            try
            {
                CatRole newrol = new CatRole() 
                {
                    Descripcion = model.Descripcion,
                };    
                _dbpContext.CatRoles.Add(newrol);
                await _dbpContext.SaveChangesAsync();  

                return StatusCode(200, new { id = newrol.Id });
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
        [Route("updateRol")]
        public async Task<ActionResult> updaterol(CatRole model)
        {
            try
            {
                _dbpContext.CatRoles.Update(model); 
                await _dbpContext.SaveChangesAsync();
                return StatusCode(200);
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
        [Route("deleteRol/{id}")]
        public async Task<ActionResult> deleterol(int id)
        {
            try
            {

                var accesos = _dbpContext.AccesosRutas.Where(x => x.IdRol == id).ToList();
                foreach (var acceso in accesos)
                {
                    _dbpContext.AccesosRutas.Remove(acceso);
                    await _dbpContext.SaveChangesAsync();
                }

                var rol = _dbpContext.CatRoles.Find(id);
                if (rol != null) 
                {
                    _dbpContext.CatRoles.Remove(rol);
                    await _dbpContext.SaveChangesAsync();
                }
                return StatusCode(200);
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
        [Route("saveAccesos")]
        public async Task<ActionResult> saveAccesos([FromForm] string jdata, [FromForm] int idr)
        {
            try
            {  

                var accesos = _dbpContext.AccesosRutas.Where(x => x.IdRol == idr).ToList();
                foreach (var acceso in accesos) 
                {
                    _dbpContext.AccesosRutas.Remove(acceso);    
                    await _dbpContext.SaveChangesAsync();
                }

                int[] intArray = JsonSerializer.Deserialize<int[]>(jdata);

                foreach (var item in intArray) 
                {
                    _dbpContext.AccesosRutas.Add(new AccesosRuta()
                    {
                        IdRol = idr,
                        IdRuta = item
                    });
                }
                await _dbpContext.SaveChangesAsync();


                return StatusCode(200);
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
}
