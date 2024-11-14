﻿using API_PEDIDOS.ModelsBD2P;
using API_PEDIDOS.ModelsDB2;
using API_PEDIDOS.ModelsDBP;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
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

        public PedidosSucursalController(ILogger<CatalogosController> logger, BD2Context db2c, DBPContext dbpc, IConfiguration configuration)
        {
            _logger = logger;
            _contextdb2 = db2c;
            _dbpContext = dbpc;
            _configuration = configuration;
            connectionStringBD2 = _configuration.GetConnectionString("DB2");
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
                                referencia = art.Refproveedor
                            };

                return StatusCode(200, query.ToList());
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
                var proveedoresdb = _dbpContext.PedSucProveedores.ToList();

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
                        data.Add(articulo);
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
        public async Task<ActionResult> addProv([FromForm] int idprov)
        {
            try
            {
                var list = _dbpContext.PedSucProveedores.Where(x => x.Codproveedor == idprov).ToList();

                if (list.Count == 0)
                {
                    _dbpContext.PedSucProveedores.Add(new PedSucProveedore()
                    {
                        Codproveedor = idprov,
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
                var prov = _dbpContext.PedSucProveedores.Where(x => x.Codproveedor == id).FirstOrDefault();

                if (prov != null)
                {
                    _dbpContext.PedSucProveedores.Remove(prov);
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
                int[] articulos = JsonConvert.DeserializeObject<int[]>(jdata);

                var items = _dbpContext.PedSucArticulos.Where(x => x.Codproveedor == idprov).ToList();

                _dbpContext.PedSucArticulos.RemoveRange(items);
                await _dbpContext.SaveChangesAsync();

                foreach (int codart in articulos)
                {
                    _dbpContext.PedSucArticulos.Add(new PedSucArticulo()
                    {
                        Codproveedor = idprov,
                        Codart = codart
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

                Boolean tienedescuento = false;
                var regdesc = _dbpContext.Descuentos.Where(x => x.Codprov == model.idprov).FirstOrDefault();
                if (regdesc != null) { tienedescuento = true; }

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
                pedido.tienedescuento = tienedescuento;
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
                     pedidosdb = _dbpContext.PedidosSucursales.Where(x => x.Fecha.Date == DateTime.Now.Date && x.Estatus.Equals("POR ACEPTAR")).ToList();
                }
                else 
                {
                    pedidosdb = _dbpContext.PedidosSucursales.Where(x => x.Fecha.Date == DateTime.Now.Date && x.Estatus.Equals("POR ACEPTAR") && x.Sucursal == idsuc).ToList();
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
        [Route("eliminarItem/{idp}/{codart}")]
        public async Task<ActionResult> eliminaritem(int idp, int codart)
        {
            try
            {
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
        public async Task<ActionResult> updateitempedsuc([FromForm] int idp,[FromForm] int codart, [FromForm] int cajas,[FromForm] double unidades)
        {
            try
            {
                var pedidodb = _dbpContext.PedidosSucursales.Where(p => p.Id == idp).FirstOrDefault();
                if (pedidodb != null)
                {
                    var pedido = JsonConvert.DeserializeObject<PedidoSuc>(pedidodb.Jdata);

                    var art = pedido.articulos.Where(x => x.codArticulo == codart).FirstOrDefault();
                    if (art != null) 
                    {
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

                var pedido = JsonConvert.DeserializeObject<PedidoSuc>(pedidodb.Jdata);
                var cajafront = _contextdb2.RemCajasfronts.Where(x => x.Cajafront == 1 && x.Idfront == int.Parse(pedido.idSucursal)).FirstOrDefault();
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
                           
                            //string querynumped = "SELECT ISNULL(MAX(NUMPEDIDO), 0) AS numero_mayor FROM [BD2_PRUEBA].dbo.PEDCOMPRACAB WHERE NUMSERIE ='" + numserie + "'";
                            string querynumped = "SELECT ISNULL(MAX(NUMPEDIDO), 0) AS numero_mayor FROM [BD2].dbo.PEDCOMPRACAB WHERE NUMSERIE ='" + numserie + "'";
                            SqlCommand command = new SqlCommand(querynumped, connection, transaccion);

                            object result = command.ExecuteScalar();
                            int numpedido = Convert.ToInt32(result);
                            numpedido++;
                            string supedido = "-" + numserie + "-" + numpedido;
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
                        
                            await transaccion.CommitAsync();

                   
                            pedido.status = 3;
                            pedidodb.Jdata = JsonConvert.SerializeObject(pedido);
                            pedidodb.Estatus = "AUTORIZADO";
                            pedidodb.Numpedido = supedido;
                            pedidodb.Datam = DateTime.Now.ToString("o");
                            _dbpContext.PedidosSucursales.Update(pedidodb);
                            await _dbpContext.SaveChangesAsync();

                            // revisar auditorias  
                            //var modificaciones = _dbpContext.Modificaciones.Where(x => x.IdPedido == idp).ToList();

                            //foreach (var item in modificaciones)
                            //{
                            //    item.PedidoSerie = numserie.ToString();
                            //    item.Numpedido = numpedido;
                            //    item.Enviado = true;
                            //    _dbpContext.Modificaciones.Update(item);
                            //    await _dbpContext.SaveChangesAsync();
                            //}

                        if (pedido.tienedescuento)
                        {
                            var pedidocab = _contextdb2.Pedcompracabs.Where(x => x.Numserie == numserie && x.Numpedido == numpedido).FirstOrDefault();
                            if (pedidocab != null)
                            {
                                if (pedido.cantidaddescuento > 0)
                                {
                                    pedidocab.Totneto = pedido.total - pedido.cantidaddescuento;
                                    pedidocab.Totdtocomercial = pedido.cantidaddescuento;

                                    double porcentajedescuento = (pedido.cantidaddescuento / pedidocab.Totbruto.Value) * 100.00;
                                    pedidocab.Dtocomercial = porcentajedescuento;

                                    _contextdb2.Pedcompracabs.Update(pedidocab);
                                    await _contextdb2.SaveChangesAsync();

                                    var pedcompratot = _contextdb2.Pedcompratots.Where(x => x.Serie == numserie && x.Numero == numpedido).FirstOrDefault();
                                    if (pedcompratot != null)
                                    {
                                        pedcompratot.Totdtocomerc = pedido.cantidaddescuento;
                                        _contextdb2.Pedcompratots.Update(pedcompratot);
                                        await _contextdb2.SaveChangesAsync();
                                    }
                                }


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

    }

    public class previewpedModel
    {
        public int idprov { get; set; }
        public int idsuc { get; set; }
        public string nombresuc { get; set;}
        public string jdataart { get; set; }
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

        public int cajas { get; set; }
        public double unidadescaja { get; set; }
        public double unidadestotales { get; set; }
        public int tipoImpuesto { get; set; }

        public double iva { get; set; }
        public double total_linea { get; set; }
        public string codigoAlmacen { get; set; }

        public Boolean tienemultiplo { get; set; }
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

        public Boolean tienedescuento { get; set; }
        public double cantidaddescuento { get; set; }

        public string? numpedido { get; set; }

    }
}
