using API_PEDIDOS.ModelsDB2;
using API_PEDIDOS.ModelsDBP;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace API_PEDIDOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AsignacionProvController : ControllerBase
    {
        private readonly ILogger<CatalogosController> _logger;
        protected BD2Context _contextdb2;
        protected DBPContext _dbpContext;


        public AsignacionProvController(ILogger<CatalogosController> logger, BD2Context db2c, DBPContext dbpc) 
        {
            _logger = logger;
            _contextdb2 = db2c;
            _dbpContext = dbpc;
        }

        [HttpGet]
        [Route("getAsignaciones")]
        public async Task<ActionResult> GetAsignaciones()
        {
            try
            {
                var query = from ap in _dbpContext.AsignacionProvs
                    join user in _dbpContext.Usuarios on ap.Idu equals user.Id   
                    select new 
                    { 
                        id = ap.Id,
                        idu = ap.Idu,
                        idprov = ap.Idprov,
                        idsuc = ap.Idsuc,
                        nombre = user.Nombre,
                        ap_paterno = user.ApellidoP,
                        ap_materno = user.ApellidoM
                    };

                var lista = query.ToList();

                List<Object> result = new List<Object>();
                foreach (var item in lista) 
                {
                    string nomprov = "";
                    String nombresucursal = ""; 
                    var proveedor = _contextdb2.Proveedores.Where(x => x.Codproveedor == item.idprov).FirstOrDefault();
                    if (proveedor != null)
                    {
                        nomprov = proveedor.Nomproveedor; 
                    }

                    var sucursal = _contextdb2.RemFronts.Where(x => x.Idfront == item.idsuc).FirstOrDefault();
                    if (sucursal != null) 
                    {
                        nombresucursal = sucursal.Titulo; 
                    }

                    result.Add(new 
                    {
                        id = item.id,
                        idu = item.idu,
                        idprov = item.idprov,
                        nombre = item.nombre,
                        ap_paterno = item.ap_paterno,
                        ap_materno = item.ap_materno,
                        nombreprov = nomprov,
                        nombresuc = nombresucursal
                    });

                }


                return StatusCode(StatusCodes.Status200OK, result);
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
        [Route("addAsignacionProv")]
        public async Task<ActionResult> addAsignacion([FromForm] int idprov,[FromForm] int idu, [FromForm] string jdsucursales)
        {
            try
            {
                int[] sucursales = JsonConvert.DeserializeObject<int[]>(jdsucursales);

                foreach (var item in sucursales)
                {

                    _dbpContext.AsignacionProvs.Add(new AsignacionProv()
                    {
                        Idprov = idprov,
                        Idu = idu,
                        Idsuc = item
                    });
                    await _dbpContext.SaveChangesAsync();

                }

                return StatusCode(StatusCodes.Status200OK);
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


        [HttpDelete]
        [Route("deleteAsignacionesProv/{jdata}")]
        public async Task<ActionResult> deleteAsignacion(string jdata)
        {
            try
            {
                int[] asignaciones = JsonConvert.DeserializeObject<int[]>(jdata);
                foreach (var item in asignaciones)
                {
                    var obj = _dbpContext.AsignacionProvs.Where(x => x.Id == item).FirstOrDefault();
                    if (obj != null)
                    {
                        _dbpContext.AsignacionProvs.Remove(obj);
                        await _dbpContext.SaveChangesAsync();
                    }
                }
               
                return StatusCode(StatusCodes.Status200OK);
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
        [Route("getAsignacionesPedSuc")]
        public async Task<ActionResult> GetAsignacionesPedSuc()
        {
            try
            {
                var query = from ap in _dbpContext.PedSucAsignaciones
                            join user in _dbpContext.Usuarios on ap.Idu equals user.Id
                            select new
                            {
                                id = ap.Id,
                                idu = ap.Idu,
                                idprov = ap.Idprov,
                                idsuc = ap.Ids,
                                nombre = user.Nombre,
                                ap_paterno = user.ApellidoP,
                                ap_materno = user.ApellidoM
                            };

                var lista = query.ToList();

                List<Object> result = new List<Object>();
                foreach (var item in lista)
                {
                    string nomprov = "";
                    String nombresucursal = "";
                    var proveedor = _contextdb2.Proveedores.Where(x => x.Codproveedor == item.idprov).FirstOrDefault();
                    if (proveedor != null)
                    {
                        nomprov = proveedor.Nomproveedor;
                    }

                    var sucursal = _contextdb2.RemFronts.Where(x => x.Idfront == item.idsuc).FirstOrDefault();
                    if (sucursal != null)
                    {
                        nombresucursal = sucursal.Titulo;
                    }

                    result.Add(new
                    {
                        id = item.id,
                        idu = item.idu,
                        idprov = item.idprov,
                        nombre = item.nombre,
                        ap_paterno = item.ap_paterno,
                        ap_materno = item.ap_materno,
                        nombreprov = nomprov,
                        nombresuc = nombresucursal
                    });

                }


                return StatusCode(StatusCodes.Status200OK, result);
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
        [Route("addAsignacionProvPedSuc")]
        public async Task<ActionResult> addAsignacionPedSuc([FromForm] int idprov, [FromForm] int idu, [FromForm] string jdsucursales)
        {
            try
            {
                int[] sucursales = JsonConvert.DeserializeObject<int[]>(jdsucursales);

                foreach (var item in sucursales)
                {

                    var reg = _dbpContext.PedSucAsignaciones.Where(x=> x.Idprov == idprov && x.Ids== item && x.Idu == idu).FirstOrDefault();
                    if (reg == null) 
                    {
                        _dbpContext.PedSucAsignaciones.Add(new PedSucAsignacione()
                        {
                            Idprov = idprov,
                            Idu = idu,
                            Ids = item,
                        });
                        await _dbpContext.SaveChangesAsync();
                    }

                }

                return StatusCode(StatusCodes.Status200OK);
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

        [HttpDelete]
        [Route("deleteAsignacionesProvPedSuc/{jdata}")]
        public async Task<ActionResult> deleteAsignacionPedSuc(string jdata)
        {
            try
            {
                int[] asignaciones = JsonConvert.DeserializeObject<int[]>(jdata);
                foreach (var item in asignaciones)
                {
                    var obj = _dbpContext.PedSucAsignaciones.Where(x => x.Id == item).FirstOrDefault();
                    if (obj != null)
                    {
                        _dbpContext.PedSucAsignaciones.Remove(obj);
                        await _dbpContext.SaveChangesAsync();
                    }
                }

                return StatusCode(StatusCodes.Status200OK);
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
