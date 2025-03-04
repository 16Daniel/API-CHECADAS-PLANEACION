
using API_PEDIDOS.ModelsDB2;
using API_PEDIDOS.ModelsDBP;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace API_PEDIDOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidosSucursalController : ControllerBase
    {
        private readonly ILogger<CatalogosController> _logger;
        protected BD2Context _contextdb2;
        protected DBPContext _dbpContext;
        private readonly IConfiguration _configuration;
        public string connectionStringBD2 = string.Empty;
        public string connectionStringBD2p = string.Empty;

        public PedidosSucursalController(ILogger<CatalogosController> logger, BD2Context db2c, DBPContext dbpc, IConfiguration configuration)
        {
            _logger = logger;
            _contextdb2 = db2c;
            _dbpContext = dbpc;
            _configuration = configuration;
            connectionStringBD2 = _configuration.GetConnectionString("DB2");
            connectionStringBD2p = _configuration.GetConnectionString("DB2P");
        }

        [HttpGet]
        [Route("getProveedores")]
        public async Task<ActionResult> GetProveedores()
        {
            try
            {
                var repository = _contextdb2.Proveedores.Where(p => p.Descatalogado == "F").Select(s => new
                {
                    codproveedor = s.Codproveedor,
                    nombre = s.Nomproveedor,
                    rfc = s.Nif20
                }).ToList();

                return StatusCode(200, repository);
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
        [Route("getItemsprov/{idprov}")]
        public async Task<ActionResult> GetItemsprovP(int idprov)
        {
            try
            {

                var query = from art in _contextdb2.Articulos1
                            join artcl in _contextdb2.Articuloscamposlibres on art.Codarticulo equals artcl.Codarticulo
                            into gj
                            from subartcl in gj.DefaultIfEmpty()
                            join prec in _contextdb2.Precioscompras on art.Codarticulo equals prec.Codarticulo
                            into gj2
                            from subprec in gj2.DefaultIfEmpty()
                            where subartcl != null && subprec.Codproveedor == idprov && art.Descatalogado == "F"
                            select new
                            {
                                cod = art.Codarticulo,
                                descripcion = art.Descripcion,
                                referencia = art.Refproveedor,
                            };

        var articulos = query.ToList();

        List<Object> data = new List<Object>();

        foreach (var item in articulos)
        {
          var artdb = _dbpContext.PedSucArticulos.Where(x => x.Codart == item.cod && x.Codproveedor == idprov).FirstOrDefault();
          data.Add(
            new
            {
              cod = item.cod,
              descripcion = item.descripcion,
              referencia = item.referencia,
              fiscal = artdb == null ? false : artdb.Fiscal,
            }
            );

        }
        return StatusCode(200,data);
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
        [Route("getProveedoresPedSuc")]
        public async Task<ActionResult> GetProveedoresPedSuc()
        {
            try
            { List<Object> data = new List<Object>();
                //var proveedoresdb = _dbpContext.PedSucProveedores.ToList();

                var proveedoresdb = _dbpContext.PedSucProveedores
    .GroupBy(p => p.Codproveedor)
    .Select(g => g.FirstOrDefault()) // Puedes obtener el primer registro de cada grupo.
    .ToList();

                foreach (var prov in proveedoresdb)
                {
                    var reg = _contextdb2.Proveedores.Where(p => p.Codproveedor == prov.Codproveedor).Select(s => new
                    {
                        codproveedor = s.Codproveedor,
                        nombre = s.Nomproveedor,
                        rfc = s.Nif20
                    }).FirstOrDefault();

                    if (reg != null)
                    {
                        data.Add(reg);
                    }
                }

                return StatusCode(200, data);
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
        [Route("getItemsprovPedSuc/{idprov}")]
        public async Task<ActionResult> GetItemsprovPedSuc(int idprov)
        {
            try
            {
                List<Object> data = new List<Object>();
                var articulosdb = _dbpContext.PedSucArticulos.Where(x => x.Codproveedor == idprov).ToList();
                var prov = _contextdb2.Proveedores.Where(x => x.Codproveedor == idprov).FirstOrDefault();

                foreach (var art in articulosdb)
                {
                    var articulo = _contextdb2.Articulos1.Where(x => x.Codarticulo == art.Codart).Select
                        (s =>
                            new
                            {
                                cod = s.Codarticulo,
                                descripcion = s.Descripcion,
                                referencia = s.Refproveedor,
                            }
                        ).FirstOrDefault();
                    if (articulo != null)
                    {
                        var itprod = _contextdb2.ItProductos.Where(p => p.Rfc == prov.Nif20 && p.Codarticulo == art.Codart).FirstOrDefault();
                         var artdb = _dbpContext.PedSucArticulos.Where(x => x.Codart == articulo.cod && x.Codproveedor == idprov).FirstOrDefault();

            Boolean tienemultiplo = itprod == null ? false : true;
                        data.Add(new
                        {
                            cod = articulo.cod,
                            descripcion = articulo.descripcion,
                            referencia = articulo.referencia,
                            tudm = tienemultiplo,
                            fiscal = artdb.Fiscal 
                        });
                    }
                }

                return StatusCode(200, data);
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
        [Route("getSucursalesProvPedSuc/{idprov}")]
        public async Task<ActionResult> GetSucursalesProvPedSuc(int idprov)
        {
            try
            {
                List<Object> data = new List<Object>();
                var list = _dbpContext.PedSucProveedores.Where(x => x.Codproveedor == idprov).ToList(); 
                foreach(var item in list) 
                {
                    var sucursaL = _contextdb2.RemFronts.Where(x => x.Idfront == item.Codsucursal).FirstOrDefault();
                    if (sucursaL != null) 
                    {
                        data.Add(new{ cod= sucursaL.Idfront,name= sucursaL.Titulo });
                    }
                }

                return StatusCode(200, data);
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
        [Route("agregarProveedor")]
        public async Task<ActionResult> addProv([FromForm] int idprov,[FromForm] string jdata)
        {
            try
            {
                int[] idsucs = JsonConvert.DeserializeObject<int[]>(jdata);
               
                var list = _dbpContext.PedSucProveedores.Where(x => x.Codproveedor == idprov).ToList();

                if (list.Count > 0)
                {
                    _dbpContext.PedSucProveedores.RemoveRange(list); 
                    await _dbpContext.SaveChangesAsync();
                }

                foreach (int ids in idsucs) 
                {
                    _dbpContext.PedSucProveedores.Add(new PedSucProveedore()
                    {
                        Codproveedor = idprov,
                        Codsucursal = ids
                    });
                    await _dbpContext.SaveChangesAsync();
                }
              
                return StatusCode(StatusCodes.Status200OK);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.ToString() });
            }


        }

        [HttpDelete("deleteProvPedSuc/{id}")]
        public async Task<IActionResult> DeleteProvPedSuc(int id)
        {

            try
            {
                var articulosprov = _dbpContext.PedSucArticulos.Where(x => x.Codproveedor == id).ToList();
                if (articulosprov.Count > 0) 
                {
                    _dbpContext.PedSucArticulos.RemoveRange(articulosprov);
                    await _dbpContext.SaveChangesAsync();
                }
                var prov = _dbpContext.PedSucProveedores.Where(x => x.Codproveedor == id).ToList(); 

                if (prov.Count> 0)
                {
                    _dbpContext.PedSucProveedores.RemoveRange(prov);
                    await _dbpContext.SaveChangesAsync();
                }

                return StatusCode(StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.ToString() });
            }

        }

        [HttpPost]
        [Route("agregarItems")]
        public async Task<ActionResult> addProvitems([FromForm] int idprov, [FromForm] string jdata)
        {
            try
            {
                List<itempProvSuc> articulos = JsonConvert.DeserializeObject<List<itempProvSuc>>(jdata);

                var items = _dbpContext.PedSucArticulos.Where(x => x.Codproveedor == idprov).ToList();

                _dbpContext.PedSucArticulos.RemoveRange(items);
                await _dbpContext.SaveChangesAsync();

                foreach (var art in articulos)
                {
                    _dbpContext.PedSucArticulos.Add(new PedSucArticulo()
                    {
                        Codproveedor = idprov,
                        Codart = art.id,
                        Fiscal = art.fiscal == null ? false : art.fiscal,  
                    });
                }
                await _dbpContext.SaveChangesAsync();

                return StatusCode(StatusCodes.Status200OK);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.ToString() });
            }


        }


        [HttpGet]
        [Route("getSucUser/{idu}")]
        public async Task<ActionResult> GetsucUser(int idu)
        {
            try
            {
                int idf = -1;
                string nombre = "";
                var sucursalreg = _dbpContext.UsuariosSucursals.Where(x => x.Idu == idu).FirstOrDefault();
                if (sucursalreg != null)
                {
                    var suc = _contextdb2.RemFronts.Where(x => x.Idfront == sucursalreg.Idsuc).FirstOrDefault();
                    if (suc != null)
                    {
                        idf = suc.Idfront;
                        nombre = suc.Titulo;
                    }
                }
                return StatusCode(200, new { cod = idf, name = nombre });
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
        [Route("PreviewPedido")]
        public async Task<ActionResult> previewpedido([FromBody] previewpedModel model)
        {
            try
            {
                int[] data = JsonConvert.DeserializeObject<int[]>(model.jdataart);

                List<ArticuloPedSuc> articulos = new List<ArticuloPedSuc>();
                int numlinea = 1; 
                foreach (var codart in data) 
                {
                    var art = _contextdb2.Articulos1.Where(x => x.Codarticulo == codart).FirstOrDefault();
                    var prov = _contextdb2.Proveedores.Where(x => x.Codproveedor == model.idprov).FirstOrDefault();
                    var preciocompra = _contextdb2.Precioscompras.Where(x => x.Codarticulo == codart && x.Codproveedor == model.idprov).FirstOrDefault(); 
                    int iva = 0;
                    var itprod = _contextdb2.ItProductos.Where(p => p.Rfc == prov.Nif20 && p.Codarticulo == codart).FirstOrDefault();
                    Boolean tienemultiplo = itprod == null ? false : true;
                    string udm = "";

                    if (itprod != null) 
                    {
                        udm = itprod.Umedida; 
                    }

                    double unidadescaja = itprod == null ? 1 : (double)itprod.Uds;
                    int cajas = 0;
                    double unidades_totales = cajas * unidadescaja;
                    double total_linea = (double)(unidades_totales * preciocompra.Pbruto);
                    var itemimpuesto = _contextdb2.Impuestos.Where(p => p.Tipoiva == art.Impuestocompra).FirstOrDefault();
                    double ivaArt = (double)(itemimpuesto.Iva == null ? 16 : itemimpuesto.Iva);

                    articulos.Add(new ArticuloPedSuc()
                    {
                        codArticulo = codart,
                        nombre = art.Descripcion,
                        precio = (double)preciocompra.Pbruto,
                        numlinea = numlinea,
                        cajas = cajas,
                        unidadescaja = unidadescaja,
                        unidadestotales = unidades_totales,
                        tipoImpuesto = (int)art.Impuestocompra,
                        iva = ivaArt,
                        total_linea = total_linea,
                        codigoAlmacen = model.idsuc.ToString(),
                        tienemultiplo = tienemultiplo,
                        udm = udm
                    });

                    numlinea++; 
                }

                return StatusCode(StatusCodes.Status200OK,articulos);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.ToString() });
            }


        }


        [HttpPost]
        [Route("GuardarPedido")]
        public async Task<ActionResult> Guadarpedido([FromBody] GuardarpedModel model)
        {
            try
            {
                var prov = _contextdb2.Proveedores.Where(x => x.Codproveedor == model.idprov).FirstOrDefault();
                List<ArticuloPedSuc> articulos = JsonConvert.DeserializeObject<List<ArticuloPedSuc>>(model.jdataart);

                double total = 0;

                foreach (ArticuloPedSuc art in articulos)
                {
                    art.unidadestotales = art.cajas * art.unidadescaja; 
                    art.total_linea = art.unidadestotales * art.precio; 
                    total = total + art.total_linea;
                }

                PedidoSuc pedido = new PedidoSuc(); 

                pedido.idSucursal = model.idsuc.ToString();
                pedido.nombresucursal = model.nombresuc;
                pedido.nombreproveedor = prov.Nomproveedor;
                pedido.codProveedor = prov.Codproveedor;
                pedido.fechaEntrega = model.fechaentrega;
                pedido.total = total;
                pedido.status = 1;
                pedido.rfc = prov.Nif20;
                pedido.nombresolicitante = model.nombresolicitante; 
                pedido.articulos = articulos;
                pedido.fecha = DateTime.Now;
                pedido.cantidaddescuento = 0; 
                string jdatapedido = JsonConvert.SerializeObject(pedido);

                _dbpContext.PedidosSucursales.Add(new PedidosSucursale()
                {
                    Sucursal = model.idsuc,
                    Proveedor = model.idprov,
                    Jdata = jdatapedido,
                    Estatus = "POR ACEPTAR",
                    Datam = null,
                    Fecha = DateTime.Now,
                    Numpedido = null
                });

               await _dbpContext.SaveChangesAsync();
                return StatusCode(StatusCodes.Status200OK);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.ToString() });
            }


        }

        [HttpGet]
        [Route("getPedidos/{idsuc}")]
        public async Task<ActionResult> GetPedidosBD(int idsuc)
        {
            try
            {
                List<PedidoSuc> pedidos = new List<PedidoSuc>();

                List<PedidosSucursale> pedidosdb = new List<PedidosSucursale>();

                if (idsuc == -1)
                {
                     pedidosdb = _dbpContext.PedidosSucursales.Where(x => (x.Fecha.Date >= DateTime.Now.Date) && x.Estatus.Equals("POR ACEPTAR")).ToList();
                }
                else 
                {
                    pedidosdb = _dbpContext.PedidosSucursales.Where(x => (x.Fecha.Date >= DateTime.Now.Date) && x.Estatus.Equals("POR ACEPTAR") && x.Sucursal == idsuc).ToList();
                }
               
                foreach (var item in pedidosdb)
                {
                    PedidoSuc p = JsonConvert.DeserializeObject<PedidoSuc>(item.Jdata);
                    p.id = item.Id;
                    pedidos.Add(p);
                }

                return StatusCode(200, pedidos);
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
    [Route("getPedidosFecha")]
    public async Task<ActionResult> GetPedidosBDFecha([FromForm] DateTime fecha, [FromForm] int ids)
    {
      try
      {
        List<PedidoSuc> pedidos = new List<PedidoSuc>();

        List<PedidosSucursale> pedidosdb = new List<PedidosSucursale>();

        if (ids == -1)
        {
          pedidosdb = _dbpContext.PedidosSucursales.Where(x => x.Fecha.Date == fecha.Date && x.Estatus.Equals("POR ACEPTAR")).ToList();
        }
        else
        {
          pedidosdb = _dbpContext.PedidosSucursales.Where(x => x.Fecha.Date == fecha.Date && x.Estatus.Equals("POR ACEPTAR") && x.Sucursal == ids).ToList();
        }

        foreach (var item in pedidosdb)
        {
          PedidoSuc p = JsonConvert.DeserializeObject<PedidoSuc>(item.Jdata);
          p.id = item.Id;
          pedidos.Add(p);
        }

        return StatusCode(200, pedidos);
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
        [Route("getPedidosH")]
        public async Task<ActionResult> GetPedidosBDH([FromForm] DateTime fechaini, [FromForm] DateTime fechafin, [FromForm] int idsuc)
        {
            try
            {
                List<PedidoSuc> pedidos = new List<PedidoSuc>();
                List<PedidosSucursale> pedidosdb = new List<PedidosSucursale>();

                if (idsuc == -1)
                {
                    pedidosdb = _dbpContext.PedidosSucursales.Where(x => x.Fecha.Date >= fechaini.Date && x.Fecha.Date <= fechafin.Date && (x.Estatus.Equals("AUTORIZADO") || x.Estatus.Equals("RECHAZADO"))).ToList();
                }
                else
                {
                    pedidosdb = _dbpContext.PedidosSucursales.Where(x => x.Fecha.Date >= fechaini.Date && x.Fecha.Date <= fechafin.Date && (x.Estatus.Equals("AUTORIZADO") || x.Estatus.Equals("RECHAZADO")) && x.Sucursal == idsuc).ToList();
                }

                foreach (var item in pedidosdb)
                {
                    PedidoSuc p = JsonConvert.DeserializeObject<PedidoSuc>(item.Jdata);
                    p.id = item.Id;
                    p.numpedido = item.Numpedido;
                    pedidos.Add(p);
                }

                return StatusCode(200, pedidos);
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
        [Route("rechazarPedido/{idp}")]
        public async Task<ActionResult> rechazar(int idp)
        {
            try
            {
                var pedidodb = _dbpContext.PedidosSucursales.Where(p => p.Id == idp).FirstOrDefault();
                if (pedidodb != null)
                {
                    var pedido = JsonConvert.DeserializeObject<PedidoSuc>(pedidodb.Jdata);
                    pedido.status = 4;
                    pedidodb.Jdata = JsonConvert.SerializeObject(pedido);
                    pedidodb.Estatus = "RECHAZADO";
                    _dbpContext.PedidosSucursales.Update(pedidodb);
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
        [Route("eliminarItem/{idp}/{codart}/{idu}")]
        public async Task<ActionResult> eliminaritem(int idp, int codart,int idu)
        {
            try
            {
                string valantes = ""; 
                var pedidodb = _dbpContext.PedidosSucursales.Where(p => p.Id == idp).FirstOrDefault();
                if (pedidodb != null)
                {
                    var pedido = JsonConvert.DeserializeObject<PedidoSuc>(pedidodb.Jdata);
                    pedido.articulos.RemoveAll(a => a.codArticulo == codart);
                    double total = 0; 
                    foreach (var articulo in pedido.articulos)
                    {
                        total = total + articulo.total_linea;
                    }
                    pedido.total = total;
                    pedidodb.Jdata = JsonConvert.SerializeObject(pedido);
                    _dbpContext.PedidosSucursales.Update(pedidodb);
                    await _dbpContext.SaveChangesAsync();

                    _dbpContext.ModificacionesPedSucs.Add(new ModificacionesPedSuc() 
                    {
                        Modificacion = "ELIMINAR LÍNEA",
                        ValAntes = valantes,
                        ValDespues = "",
                        Justificacion = "LÍNEA INNECESARIA",
                        Fecha = DateTime.Now,
                        Idusuario = idu,
                        Idpedido = idp,
                        Codart = codart,
                        Enviado = false,
                        Comentario = ""
                    });

                    await _dbpContext.SaveChangesAsync();   
                }
                return StatusCode(StatusCodes.Status200OK);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.ToString() });
            }


        }


        [HttpPost]
        [Route("updateitemPedSuc")]
        public async Task<ActionResult> updateitempedsuc([FromForm] int idp,[FromForm] int codart, [FromForm] int cajas,[FromForm] double unidades,[FromForm]int idu, [FromForm] string justificacion, [FromForm] string comentario)
        {
            try
            {
                string valantes = ""; 
                var pedidodb = _dbpContext.PedidosSucursales.Where(p => p.Id == idp).FirstOrDefault();
                if (pedidodb != null)
                {
                    var pedido = JsonConvert.DeserializeObject<PedidoSuc>(pedidodb.Jdata);

                    var art = pedido.articulos.Where(x => x.codArticulo == codart).FirstOrDefault();
                    if (art != null) 
                    {
                        valantes = art.unidadestotales.ToString(); 
                        art.cajas = cajas; 
                        art.unidadestotales = unidades;
                        art.total_linea = (art.unidadestotales * art.precio); 
                    }
                    double total = 0; 
                    foreach (var articulo in pedido.articulos) 
                    {
                        total = total + articulo.total_linea;   
                    }

                    pedido.total = total; 
                    pedidodb.Jdata = JsonConvert.SerializeObject(pedido);
                    _dbpContext.PedidosSucursales.Update(pedidodb);
                    await _dbpContext.SaveChangesAsync();

                    _dbpContext.ModificacionesPedSucs.Add(new ModificacionesPedSuc()
                    {
                        Modificacion = "EDITAR LÍNEA",
                        ValAntes = valantes,
                        ValDespues = unidades.ToString(),
                        Justificacion = justificacion,
                        Fecha = DateTime.Now,
                        Idusuario = idu,
                        Idpedido = idp,
                        Codart = codart,
                        Enviado = false,
                        Comentario = comentario
                    });

                    await _dbpContext.SaveChangesAsync();
                }
                return StatusCode(StatusCodes.Status200OK);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.ToString() });
            }

        }


          [HttpGet]
        [Route("ConfirmarPedido/{idp}")]
        public async Task<ActionResult> confirmarPedido(int idp)
        {
            try
            {
                
                var pedidodb = _dbpContext.PedidosSucursales.Find(idp);
                SqlConnection connection = (SqlConnection)_dbpContext.Database.GetDbConnection();
                connection.Open();

                SqlTransaction transaccion = connection.BeginTransaction();

                var remfront = _contextdb2.RemFronts.Where(x => x.Idfront == pedidodb.Sucursal).FirstOrDefault(); 

                var pedido = JsonConvert.DeserializeObject<PedidoSuc>(pedidodb.Jdata);
                var cajafront = _contextdb2.RemCajasfronts.Where(x => x.Cajafront == 1 && x.Idfront == int.Parse(pedido.idSucursal)).FirstOrDefault();
                var codcliente = remfront.Codcliente; 
                var transporte = _contextdb2.Transportes.Where(x => x.Fax == pedidodb.Sucursal.ToString()).FirstOrDefault();
                int idtransporte = 0;
                if (transporte != null) { idtransporte = transporte.Codigo; }
                string numserie = cajafront.Cajamanager + "X";

                string codalmacen = "";

                if (int.Parse(pedido.idSucursal) < 10)
                {
                    codalmacen = "0" + pedido.idSucursal;
                }
                else { codalmacen = pedido.idSucursal; }

                    try
                    {
                        if (pedidodb == null)
                        {
                            connection.Close();
                            return StatusCode(StatusCodes.Status404NotFound);
                        }
                        else
                        {
            if (pedidodb.Numpedido == null)
            {
              //string querynumped = "SELECT ISNULL(MAX(NUMPEDIDO), 0) AS numero_mayor FROM [BD2_PRUEBA].dbo.PEDCOMPRACAB WHERE NUMSERIE ='" + numserie + "'";
              string querynumped = "SELECT ISNULL(MAX(NUMPEDIDO), 0) AS numero_mayor FROM [BD2].dbo.PEDCOMPRACAB WHERE NUMSERIE ='" + numserie + "'";
              SqlCommand command = new SqlCommand(querynumped, connection, transaccion);

              object result = command.ExecuteScalar();
              int numpedido = Convert.ToInt32(result);
              numpedido++;
              string supedido = "-" + numserie + "-" + numpedido;
              string csupedido = numserie + "-" + numpedido;
              double totalimpuestos = 0;

              foreach (var item in pedido.articulos)
              {
                totalimpuestos += (item.total_linea * item.iva) / 100;
              }

              // insertar pedcompracab
              command = new SqlCommand("SP_INSERT_PEDIDO", connection, transaccion);
              command.CommandType = CommandType.StoredProcedure;
              // Parámetros del procedimiento almacenado
              command.Parameters.AddWithValue("@PEDCAB_NUMSERIE", numserie);
              command.Parameters.AddWithValue("@PEDCAB_NUMPEDIDO", numpedido);
              command.Parameters.AddWithValue("@PEDCAB_CODPROVEEDOR", pedido.codProveedor);
              command.Parameters.AddWithValue("@PEDCAB_FECHA_PEDIDO", DateTime.Now);
              command.Parameters.AddWithValue("@PEDCAB_FECHA_ENTREGA", pedido.fechaEntrega);
              command.Parameters.AddWithValue("@PEDCAB_TOTBRUTO", pedido.total);
              command.Parameters.AddWithValue("@PEDCAB_TOTIMPUESTOS", totalimpuestos);
              command.Parameters.AddWithValue("@PEDCAB_TOTNETO", pedido.total + totalimpuestos);
              command.Parameters.AddWithValue("@PEDCAB_SUPEDIDO", supedido);
              command.Parameters.AddWithValue("@TRANSPORTE", idtransporte);
              command.ExecuteNonQuery();

              // insertar pedcompralin
              int numlinea = 0;
              foreach (var art in pedido.articulos)
              {
                numlinea++;
                var articulodb = _contextdb2.Articulos1.Where(x => x.Codarticulo == art.codArticulo).FirstOrDefault();
                string referencia = articulodb.Refproveedor;
                command = new SqlCommand("SP_INSERT_PEDIDOLIN", connection, transaccion);
                command.CommandType = CommandType.StoredProcedure;
                // Agregar parámetros
                command.Parameters.AddWithValue("@PEDLIN_NUMSERIE", numserie);
                command.Parameters.AddWithValue("@PEDLIN_NUMPEDIDO", numpedido);
                command.Parameters.AddWithValue("@PEDLIN_NUMLINEA", numlinea);
                command.Parameters.AddWithValue("@PEDLIN_CODARTICULO", art.codArticulo);
                command.Parameters.AddWithValue("@PEDLIN_REFERENCIA", referencia);
                command.Parameters.AddWithValue("@PEDLIN_DESCRIPCION", articulodb.Descripcion);
                command.Parameters.AddWithValue("@PEDLIN_CAJAS", art.cajas);
                command.Parameters.AddWithValue("@PEDLIN_UNIDADES", art.unidadescaja);
                command.Parameters.AddWithValue("@PEDLIN_UDSTOTALES", art.unidadestotales);
                command.Parameters.AddWithValue("@PEDLIN_PRECIO", art.precio);
                command.Parameters.AddWithValue("@PEDLIN_TIPOIMPUESTO", art.tipoImpuesto);
                command.Parameters.AddWithValue("@PEDLIN_IVA", art.iva);
                command.Parameters.AddWithValue("@PEDLIN_IEPS", 0);
                command.Parameters.AddWithValue("@PEDLIN_TOTAL", art.total_linea);
                command.Parameters.AddWithValue("@PEDLIN_CODALMACEN", codalmacen);
                command.Parameters.AddWithValue("@PEDLIN_SUPEDIDO", supedido);
                command.Parameters.AddWithValue("@PEDLIN_FECHAENTREGA", pedido.fechaEntrega);
                command.ExecuteNonQuery();
              }

              // insertar pedcompratot
              int numlineatot = 0;
              var impuestos = pedido.articulos.Select(articulo => articulo.tipoImpuesto).Distinct();
              foreach (var impuesto in impuestos)
              {
                numlineatot++;
                double totalbruto = 0;
                var articulosimp = pedido.articulos.Where(x => x.tipoImpuesto == impuesto);
                double iva = 0;
                foreach (var item in articulosimp)
                {
                  totalbruto += item.total_linea;
                  iva = item.iva;
                }

                command = new SqlCommand("SP_INSERT_COMPRATOT", connection, transaccion);
                command.CommandType = CommandType.StoredProcedure;
                // Agregar parámetros al procedimiento almacenado
                command.Parameters.AddWithValue("@PEDTOT_NUMSERIE", numserie);
                command.Parameters.AddWithValue("@PEDTOT_NUMPEDIDO", numpedido);
                command.Parameters.AddWithValue("@PEDTOT_NUMLINEA", numlineatot);
                command.Parameters.AddWithValue("@PEDTOT_BRUTO", totalbruto);
                command.Parameters.AddWithValue("@PEDTOT_IVA", iva);
                command.Parameters.AddWithValue("@PEDTOT_TOTIVA", (totalbruto * iva) / 100);
                command.Parameters.AddWithValue("@PEDTOT_IEPS", 0);
                command.Parameters.AddWithValue("@PEDTOT_TOTREQ", 0);
                command.Parameters.AddWithValue("@PEDTOT_TOTAL", totalbruto + ((totalbruto * iva) / 100));
                // Ejecutar el procedimiento almacenado
                command.ExecuteNonQuery();

              }

              // insertar tesoreria

              var prov = _contextdb2.Proveedores.Where(x => x.Codproveedor == pedido.codProveedor).FirstOrDefault();
              var fpagoprov = _contextdb2.Fpagoproveedors.Where(x => x.Codproveedor == pedido.codProveedor).FirstOrDefault();
              command = new SqlCommand("SP_INSERT_TESORERIA", connection, transaccion);
              command.CommandType = CommandType.StoredProcedure;

              // Agregar parámetros al procedimiento almacenado
              command.Parameters.AddWithValue("@TES_NUMSERIE", numserie);
              command.Parameters.AddWithValue("@TES_NUMPEDIDO", numpedido);
              command.Parameters.AddWithValue("@TES_CUENTA", prov.Codcontable);
              command.Parameters.AddWithValue("@TES_CODPROV", prov.Codproveedor);
              command.Parameters.AddWithValue("@TES_IMPORTE", pedido.total + totalimpuestos);
              command.Parameters.AddWithValue("@TES_FORMAPAGO", fpagoprov.Codformapago);
              command.Parameters.AddWithValue("@TES_FECHAVENCIMIENTO", DateTime.Now);
              command.CommandTimeout = 120;
              command.ExecuteNonQuery();

              // update seriesdoc

              command = new SqlCommand("SP_UPDATE_SERIESDOC", connection, transaccion);
              command.CommandType = CommandType.StoredProcedure;

              // Agregar parámetros al procedimiento almacenado

              ///--------------- PRUEBAS -------------------
              ///
              //command.Parameters.AddWithValue("@SERIE", "IOGFYTJDFGHJK");
              command.Parameters.AddWithValue("@SERIE", numserie);
              // Ejecutar el procedimiento almacenado
              command.ExecuteNonQuery();


              if (prov.Codproveedor == 5 || prov.Codproveedor == 1)
              {
                command = new SqlCommand("SP_INSERT_INCIDENCIA", connection, transaccion);

                command.CommandType = CommandType.StoredProcedure;

                // Agregar los parámetros
                command.Parameters.AddWithValue("@FECHA", DateTime.Now.Date);
                command.Parameters.AddWithValue("@CODCLIENTE", codcliente);
                command.Parameters.AddWithValue("@SERIE", numserie);
                command.Parameters.AddWithValue("@NUMPEDIDO", numpedido);
                command.Parameters.AddWithValue("@FECHAENTREGA", pedido.fechaEntrega.Date);
                command.Parameters.AddWithValue("@CSUPEDIDO", csupedido);
                command.Parameters.AddWithValue("@CODPROV", pedido.codProveedor);
                command.Parameters.AddWithValue("@COMENTARIOLIBRE", "");
                command.Parameters.AddWithValue("@TOTALSINIVA", pedido.total);
                command.Parameters.AddWithValue("@TOTALCONIVA", pedido.total + totalimpuestos);
                command.Parameters.AddWithValue("@IDF", pedido.idSucursal);

                command.ExecuteNonQuery();

                command = new SqlCommand("[dbo].[GET_IDINCIDENCIA]", connection, transaccion);
                command.CommandType = CommandType.StoredProcedure;

                object result2 = command.ExecuteScalar();
                int idincidencia = Convert.ToInt32(result2);

                numlinea = 0;
                foreach (var art in pedido.articulos)
                {
                  numlinea++;
                  var articulodb = _contextdb2.Articulos1.Where(x => x.Codarticulo == art.codArticulo).FirstOrDefault();
                  command = new SqlCommand("SP_INSERT_INCIDENCIA_LIN", connection, transaccion);

                  command.CommandType = CommandType.StoredProcedure;

                  // Agregar los parámetros
                  command.Parameters.AddWithValue("@IDINCIDENCIA", idincidencia);
                  command.Parameters.AddWithValue("@NUMLINEA", numlinea);
                  command.Parameters.AddWithValue("@CODART", art.codArticulo);
                  command.Parameters.AddWithValue("@UNIDADES", art.cajas);
                  command.Parameters.AddWithValue("@UNIDADES2", art.unidadescaja);
                  command.Parameters.AddWithValue("@TOTALLINEA", art.total_linea);
                  command.Parameters.AddWithValue("@DESCRIPCIONART", articulodb.Descripcion);
                  command.Parameters.AddWithValue("@CODBARRAS", "");

                  command.ExecuteNonQuery();
                }
              }

              await transaccion.CommitAsync();


              pedido.status = 3;
              pedidodb.Jdata = JsonConvert.SerializeObject(pedido);
              pedidodb.Estatus = "AUTORIZADO";
              pedidodb.Numpedido = supedido;
              pedidodb.Datam = DateTime.Now.ToString("o");
              _dbpContext.PedidosSucursales.Update(pedidodb);
              await _dbpContext.SaveChangesAsync();

              var modificaciones = _dbpContext.ModificacionesPedSucs.Where(x => x.Idpedido == idp).ToList();
              foreach (var item in modificaciones)
              {
                item.Enviado = true;
                _dbpContext.ModificacionesPedSucs.Update(item);
                await _contextdb2.SaveChangesAsync();
              }

            }

          }

                    connection.Close();

              
                    return StatusCode(StatusCodes.Status200OK);
                    }
                    catch (Exception err) 
                    {
                    await transaccion.RollbackAsync(); 
                        return StatusCode(StatusCodes.Status500InternalServerError, new { message = err.ToString() });
                    }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.ToString() });
            }

        }


        [HttpGet]
        [Route("getItemsDisponibles/{idp}")]
        public async Task<ActionResult> itemsdisponibelsped(int idp)
        {
            try
            {
                List<ArticuloPedSuc> articulos = new List<ArticuloPedSuc>();
                var pedidodb = _dbpContext.PedidosSucursales.Where(x => x.Id == idp).FirstOrDefault();
                var pedido = JsonConvert.DeserializeObject<PedidoSuc>(pedidodb.Jdata);
                List<ItemModelPS> itemsdisponibles = new List<ItemModelPS>();
                if (pedidodb != null) 
                {
                    List<ItemModelPS> data = new List<ItemModelPS>();
                    var articulosdb = _dbpContext.PedSucArticulos.Where(x => x.Codproveedor == pedido.codProveedor).ToList();
                    var prov = _contextdb2.Proveedores.Where(x => x.Codproveedor == pedido.codProveedor).FirstOrDefault();

                    foreach (var art in articulosdb)
                    {
                        var articulo = _contextdb2.Articulos1.Where(x => x.Codarticulo == art.Codart).Select
                            (s =>
                                new
                                {
                                    cod = s.Codarticulo,
                                    descripcion = s.Descripcion,
                                    referencia = s.Refproveedor,
                                }
                            ).FirstOrDefault();
                        if (articulo != null)
                        {
                            var itprod = _contextdb2.ItProductos.Where(p => p.Rfc == prov.Nif20 && p.Codarticulo == art.Codart).FirstOrDefault();
                            Boolean tienemultiplo = itprod == null ? false : true;
                            data.Add(new ItemModelPS()
                            {
                                cod = articulo.cod,
                                descripcion = articulo.descripcion,
                                referencia = articulo.referencia,
                            });
                        }
                    }

                    foreach (var item in data) 
                    {
                        if(pedido.articulos.Any(x => x.codArticulo == item.cod) == false) 
                        {
                            itemsdisponibles.Add(new ItemModelPS()
                            {
                                cod = item.cod,
                                descripcion = item.descripcion,
                                referencia = item.referencia
                            });
                        }
                    }
                    int numlinea = 1;
                    foreach (var item in itemsdisponibles)
                    {
                        var art = _contextdb2.Articulos1.Where(x => x.Codarticulo == item.cod).FirstOrDefault();
                        var preciocompra = _contextdb2.Precioscompras.Where(x => x.Codarticulo == item.cod && x.Codproveedor == pedido.codProveedor).FirstOrDefault();
                        int iva = 0;
                        var itprod = _contextdb2.ItProductos.Where(p => p.Rfc == prov.Nif20 && p.Codarticulo == item.cod).FirstOrDefault();
                        Boolean tienemultiplo = itprod == null ? false : true;
                        string udm = "";

                        if (itprod != null)
                        {
                            udm = itprod.Umedida;
                        }

                        if (preciocompra != null && itprod != null)
                        {
                          double unidadescaja = itprod == null ? 1 : (double)itprod.Uds;
                          int cajas = 0;
                          double unidades_totales = cajas * unidadescaja;
                          double total_linea = (double)(unidades_totales * preciocompra.Pbruto);
                          var itemimpuesto = _contextdb2.Impuestos.Where(p => p.Tipoiva == art.Impuestocompra).FirstOrDefault();
                          double ivaArt = (double)(itemimpuesto.Iva == null ? 16 : itemimpuesto.Iva);

                          articulos.Add(new ArticuloPedSuc()
                          {
                            codArticulo = item.cod,
                            nombre = art.Descripcion,
                            precio = (double)preciocompra.Pbruto,
                            numlinea = numlinea,
                            cajas = cajas,
                            unidadescaja = unidadescaja,
                            unidadestotales = unidades_totales,
                            tipoImpuesto = (int)art.Impuestocompra,
                            iva = ivaArt,
                            total_linea = total_linea,
                            codigoAlmacen = pedidodb.Sucursal.ToString(),
                            tienemultiplo = tienemultiplo,
                            udm = udm
                          });

                          numlinea++;
                        }           
                    }

                }   
                

                return StatusCode(StatusCodes.Status200OK,articulos);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.ToString() });
            }


        }


        [HttpPost]
        [Route("addItemPed")]
        public async Task<ActionResult> addItemPed([FromForm] int idp,[FromForm] string items, [FromForm] int idu)
        {
            try
            {
                List<ArticuloPedSuc> listitems = JsonConvert.DeserializeObject<List<ArticuloPedSuc>>(items);
                var pedidodb = _dbpContext.PedidosSucursales.Where(p => p.Id == idp).FirstOrDefault();
                if (pedidodb != null)
                {
                    var pedido = JsonConvert.DeserializeObject<PedidoSuc>(pedidodb.Jdata);

                    foreach(var item in listitems) 
                    {
                        item.unidadestotales = item.cajas * item.unidadescaja;
                        item.total_linea = (item.unidadestotales * item.precio);
                        pedido.articulos.Add(item);

                        _dbpContext.ModificacionesPedSucs.Add(new ModificacionesPedSuc()
                        {
                            Modificacion = "AGREGAR LÍNEA",
                            ValAntes = "",
                            ValDespues = item.unidadestotales.ToString(),
                            Justificacion = "",
                            Fecha = DateTime.Now,
                            Idusuario = idu,
                            Idpedido = idp,
                            Codart = item.codArticulo,
                            Enviado = false,
                            Comentario = ""
                        });

                        await _dbpContext.SaveChangesAsync();
                    }
                       
                    double total = 0;
                    foreach (var articulo in pedido.articulos)
                    {
                        total = total + articulo.total_linea;
                    }

                    pedido.total = total;
                    pedidodb.Jdata = JsonConvert.SerializeObject(pedido);
                    _dbpContext.PedidosSucursales.Update(pedidodb);
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

  public class itempProvSuc
  {
    public int id { get; set; }
    public Boolean? fiscal { get; set; }

  }
    public class previewpedModel
    {
        public int idprov { get; set; }
        public int idsuc { get; set; }
        public string nombresuc { get; set;}
        public string jdataart { get; set; }
    }

    public class ItemModelPS
    {
        public int cod { get; set; }
        public string descripcion { get; set; }
        public string referencia { get; set; }
    }

    public class GuardarpedModel
    {
        public int idprov { get; set; }
        public int idsuc { get; set; }
        public string nombresuc { get; set; }
        public string jdataart { get; set; }
        public DateTime fechaentrega {  get; set; }
        public string nombresolicitante {  get; set; }
    }

    public class ArticuloPedSuc
    {
        public int codArticulo { get; set; }
        public string nombre { get; set; }
        public double precio { get; set; }
        public int numlinea { get; set; }

        public double cajas { get; set; }
        public double unidadescaja { get; set; }
        public double unidadestotales { get; set; }
        public int tipoImpuesto { get; set; }

        public double iva { get; set; }
        public double total_linea { get; set; }
        public string codigoAlmacen { get; set; }

        public Boolean tienemultiplo { get; set; }
        public string udm {  get; set; }    
    }

    public class PedidoSuc
    {
        public int id { get; set; }
        public string idSucursal { get; set; }
        public string nombresucursal { get; set; }
        public string nombreproveedor { get; set; }
        public int codProveedor { get; set; }

        public DateTime fecha { get; set; }
        public DateTime fechaEntrega { get; set; }

        public double total { get; set; }
        public int status { get; set; }

        public string rfc { get; set; }
        public List<ArticuloPedSuc> articulos { get; set; }

        public string nombresolicitante { get; set; }

        public double cantidaddescuento { get; set; }

        public string? numpedido { get; set; }

    }
}
