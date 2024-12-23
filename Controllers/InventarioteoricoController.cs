using API_PEDIDOS.ModelsDB2;
using API_PEDIDOS.ModelsDBP;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace API_PEDIDOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventarioteoricoController : ControllerBase
    {
        private readonly ILogger<CatalogosController> _logger;
        protected BD2Context _contextdb2;
        protected DBPContext _dbpContext;
        private readonly IConfiguration _configuration;
        public string connectionStringBD2 = string.Empty;

        public InventarioteoricoController(ILogger<CatalogosController> logger, BD2Context db2c, DBPContext dbpc, IConfiguration configuration)
        {
            _logger = logger;
            _contextdb2 = db2c;
            _dbpContext = dbpc;
            _configuration = configuration;
            connectionStringBD2 = _configuration.GetConnectionString("DB2");
        }

        [HttpGet]
        [Route("getSucsinvt")]
        public async Task<ActionResult> GetSucsInvTeorico()
        {
            try
            {
                List<Object> list = new List<Object>(); 
               var datadb = _dbpContext.InventarioTeoricos.ToList();

                foreach (var item in datadb) 
                {
                    var suc = _contextdb2.RemFronts.Where(x => x.Idfront == item.Idfront).FirstOrDefault(); 
                    if (suc != null) 
                    {
                        list.Add(new { id = item.Id, idfront = item.Idfront, nombresuc = suc.Titulo });
                    }
                }

                return StatusCode(200,list);
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
        [Route("getProvsSuc/{ids}")]
        public async Task<ActionResult> GetprovSucinvteorico(int ids)
        {
            try
            {
                List<Object> list = new List<Object>();
                var datadb = _dbpContext.InvTeoricoProveedores.Where(x => x.Idfront == ids).ToList();

                foreach (var item in datadb)
                {
                    var prov = _contextdb2.Proveedores.Where(x =>x.Codproveedor == item.Codprov).FirstOrDefault();
                    if (prov != null)
                    {
                        list.Add(new
                        {
                            codproveedor = prov.Codproveedor,
                            nombre = prov.Nomproveedor,
                            rfc = prov.Nif20
                        });
                    }
                }

                return StatusCode(200, list);
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
        [Route("addSuc/{idf}/{jdata}")]
        public async Task<ActionResult> addsuc(int idf, string jdata)
        {
            try
            {
                var proveedores = _dbpContext.InvTeoricoProveedores.Where(x => x.Idfront == idf).ToList();
                if (proveedores.Count > 0)
                {
                    _dbpContext.InvTeoricoProveedores.RemoveRange(proveedores);
                    await _dbpContext.SaveChangesAsync();
                }

                int[] provs = JsonConvert.DeserializeObject<int[]>(jdata);

                foreach (int idp in provs) 
                {
                    _dbpContext.InvTeoricoProveedores.Add(new InvTeoricoProveedore() 
                    {
                        Idfront = idf,
                        Codprov = idp
                    });
                    await _dbpContext.SaveChangesAsync();
                }

                var reg = _dbpContext.InventarioTeoricos.Where(x => x.Idfront == idf).FirstOrDefault();
                if (reg == null) 
                {
                    _dbpContext.InventarioTeoricos.Add(new InventarioTeorico() { Idfront = idf });
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

        [HttpGet]
        [Route("deleteSuc/{idf}")]
        public async Task<ActionResult> deletesuc(int idf)
        {
            try
            {
                var proveedores = _dbpContext.InvTeoricoProveedores.Where(x => x.Idfront == idf).ToList();
                if (proveedores.Count > 0) 
                {
                    _dbpContext.InvTeoricoProveedores.RemoveRange(proveedores); 
                    await _dbpContext.SaveChangesAsync();   
                }
                var reg = _dbpContext.InventarioTeoricos.Where(x => x.Idfront == idf).FirstOrDefault();
                if (reg != null) 
                {
                    _dbpContext.InventarioTeoricos.Remove(reg);
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



    }
}
