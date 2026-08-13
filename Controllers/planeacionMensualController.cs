using API_PEDIDOS.ModelsBD2Prueba;
using API_PEDIDOS.ModelsDB2;
using API_PEDIDOS.ModelsDBP;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;

namespace API_PEDIDOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class planeacionMensualController : ControllerBase
    {
        private readonly ILogger<planeacionMensualController> _logger;
        protected BD2Context _contextdb2;
        protected DBPContext _dbpContext;
        private readonly CalculadoraPedidos _calculadora;
        public planeacionMensualController(ILogger<planeacionMensualController> logger, BD2Context db2c, DBPContext dbpc)
        {
            _logger = logger;
            _contextdb2 = db2c;
            _dbpContext = dbpc;
            _calculadora = new CalculadoraPedidos(_dbpContext, _contextdb2);
        }

        [HttpGet]
        [Route("getArticulos")]
        public async Task<ActionResult> GetArticulos()
        {
            try
            {
                var query = _contextdb2.Articulos
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
                                && x.artcl != null && x.artcl.InvMensual == "T")
                    .Select(x => new { x.art.Codarticulo, x.art.Descripcion });

                //var articulos = _contextdb2.Articulos1.Where(x => x.Descatalogado == "F" && !x.Descripcion.StartsWith("*")).ToList();
                var articulos = query.ToList();
                List<object> data = new List<object>();
                foreach (var articulo in articulos)
                {
                    data.Add(new { cod = articulo.Codarticulo, descripcion = articulo.Descripcion, marca = "" });
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
        public async Task<ActionResult> agregarArticulos([FromForm] string jdata, [FromForm] int codprov)
        {
            try
            {
                int[] articulos = System.Text.Json.JsonSerializer.Deserialize<int[]>(jdata);

                foreach (int art in articulos)
                {
                    var artbd = _dbpContext.CheckPlaneacionMensuals.Where(x => x.Codarticulo == art).FirstOrDefault();
                    if (artbd == null)
                    {
                        _dbpContext.CheckPlaneacionMensuals.Add(new CheckPlaneacionMensual() { Codarticulo = art, Codproveedor = codprov });
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

        [HttpGet]
        [Route("geArticulosbd")]
        public async Task<ActionResult> GetArticulosBD()
        {
            try
            {
                List<Object> dataart = new List<Object>();
                var data = _dbpContext.CheckPlaneacionMensuals.ToList();
                foreach (var item in data)
                {
                    var art = _contextdb2.Articulos.Where(x => x.Codarticulo == item.Codarticulo).FirstOrDefault();
                    var marca = _contextdb2.Marcas.Where(x => x.Codmarca == art.Marca).FirstOrDefault();
                    string nomseccion = "";
                    if (marca != null) { nomseccion = marca.Descripcion; }
                    if (art != null)
                    {
                        var prov = _contextdb2.Proveedores.Where(x => x.Codproveedor == item.Codproveedor).FirstOrDefault();
                        string nombreprov = "";
                        if (prov != null) { nombreprov = prov.Nomproveedor; }
                        dataart.Add(new { cod = art.Codarticulo, descripcion = art.Descripcion, marca = nomseccion, referencia = art.Refproveedor, umedida = art.Unidadmedida, nomprov = nombreprov });
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

        [HttpGet]
        [Route("getParametros")]
        public async Task<ActionResult> GetParametros()
        {
            try
            {
                var parametros = _dbpContext.ParametrosPedidosMensuales.FirstOrDefault();
                return Ok(parametros);
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
        [Route("getProveedoresArt/{cod}")]
        public async Task<ActionResult> GetProveedoresArt(int cod)
        {
            try
            {
                List<Object> data = new List<object>();
                var precios = _contextdb2.Precioscompras.Where(x => x.Codarticulo == cod).ToList();
                foreach (var precio in precios)
                {
                    var proveedor = _contextdb2.Proveedores.Where(x => x.Codproveedor == precio.Codproveedor).FirstOrDefault();
                    if (proveedor != null)
                    { if (proveedor.Descatalogado == "F")
                        {
                            data.Add(new { codprov = proveedor.Codproveedor, nombreprov = proveedor.Nomproveedor, rfc = proveedor.Nif20 });
                        }
                    }
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
        [Route("guardarParametros")]
        public async Task<ActionResult> GuardarParametros([FromForm] int tiempoentrega, [FromForm] int periodorev,
            [FromForm] double nivelRev, [FromForm] int meses, [FromForm] string datadivision )
        {
            try
            {
                var parametros = _dbpContext.ParametrosPedidosMensuales.FirstOrDefault();
                if (parametros != null)
                {
                    parametros.TiempoDeEntrega = tiempoentrega;
                    parametros.PeriodoDeRevision = periodorev;
                    parametros.NivelDeServicio = nivelRev;
                    parametros.MesesConDatos = meses;
                    parametros.DataDivisionPedidos = datadivision; 

                    _dbpContext.ParametrosPedidosMensuales.Update(parametros);
                    await _dbpContext.SaveChangesAsync();
                }
                else
                {
                    var parametrosobj = new ParametrosPedidosMensuale();
                    parametrosobj.TiempoDeEntrega = tiempoentrega;
                    parametrosobj.PeriodoDeRevision = periodorev;
                    parametrosobj.NivelDeServicio = nivelRev;
                    parametrosobj.MesesConDatos = meses;
                    parametrosobj.DataDivisionPedidos = null; 

                    _dbpContext.ParametrosPedidosMensuales.Add(parametrosobj);
                    await _dbpContext.SaveChangesAsync();

                }
                return Ok(parametros);
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
                    var artdb = _dbpContext.CheckPlaneacionMensuals.Where(x => x.Codarticulo == art).FirstOrDefault();
                    if (artdb != null)
                    {
                        _dbpContext.CheckPlaneacionMensuals.Remove(artdb);
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
        [Route("getProveedoresBD")]
        public async Task<ActionResult> getProveedoresBD()
        {
            try
            {
                var arr = _dbpContext.CheckPlaneacionMensuals.Select(x=>x.Codproveedor).Distinct().ToList();
                var data = new List<Object>();
                foreach (var cod in arr) 
                {
                    var prov = _contextdb2.Proveedores.Where(x=> x.Codproveedor == cod).FirstOrDefault();
                    data.Add(new { codprov = prov.Codproveedor, nombreprov = prov.Nomproveedor, rfc = prov.Nif20 });
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
        [Route("getPedidosBD")]
        public async Task<ActionResult> getPedidosBD()
        {
            try
            {
                var data = new List<ConsumoMensualGrupo>(); 
                var pedidoscab = _dbpContext.PedidosMensualCabs.Where(x => x.Fecha.Month == DateTime.Now.Month && x.Estatus == "POR ACEPTAR").ToList();
                foreach (var pc in pedidoscab) 
                {
                    var items = _dbpContext.PedidosMensualLins.Where(x => x.Idcab == pc.Id).ToList();
                    data.Add(new ConsumoMensualGrupo()
                    {
                        id = pc.Id,
                        codProveedor = pc.Codproveedor,
                        idSucursal = pc.Idsucursal,
                        fecha = pc.Fecha,
                        Items = items,
                        estatus = pc.Estatus,
                        division = pc.DivisionPedidos
                    });
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
        [Route("getPedidosBDH")]
        public async Task<ActionResult> getPedidosBDH([FromForm] DateTime fi, [FromForm] DateTime ff)
        {
            try
            {
                var data = new List<ConsumoMensualGrupo>();
                var pedidoscab = _dbpContext.PedidosMensualCabs.Where(x => x.Fecha.Date >= fi.Date && x.Fecha.Date <= ff.Date && x.Estatus == "AUTORIZADO").ToList();
                foreach (var pc in pedidoscab)
                {
                    var items = _dbpContext.PedidosMensualLins.Where(x => x.Idcab == pc.Id).ToList();
                    data.Add(new ConsumoMensualGrupo()
                    {
                        id = pc.Id,
                        codProveedor = pc.Codproveedor,
                        idSucursal = pc.Idsucursal,
                        fecha = pc.Fecha,
                        Items = items,
                        estatus = pc.Estatus,
                        division = pc.DivisionPedidos
                    });
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

        [HttpPost("calcularResumen")]
        public async Task<ActionResult> CalcularResumen()
        {
            var pedidosbd = _dbpContext.PedidosMensualCabs.Where(x => x.Fecha.Month == DateTime.Now.Month && x.Estatus == "POR ACEPTAR").ToList(); 
            _dbpContext.PedidosMensualCabs.RemoveRange(pedidosbd);
            await _dbpContext.SaveChangesAsync(); 
            var request = new SolicitudCalculo();
            var parametrosbd = _dbpContext.ParametrosPedidosMensuales.FirstOrDefault();
            ParametrosCalculo parametros = new ParametrosCalculo();
            parametros.PeriodoRevisionDias = parametrosbd.PeriodoDeRevision;
            parametros.N = parametrosbd.MesesConDatos;
            parametros.Z = parametrosbd.NivelDeServicio;
            parametros.LeadTimeDias = parametrosbd.TiempoDeEntrega;
            parametros.segmentacionpedidos = JsonConvert.DeserializeObject<List<DivisionPedidos>>(parametrosbd.DataDivisionPedidos);
            request.Parametros = parametros;

            List<ConsumoMensualInput> datosinput = new List<ConsumoMensualInput>();

            var sucursales = await _calculadora.GetSucursales();
            //sucursales = sucursales.GetRange(0, 2); 
            var articulosbd = _dbpContext.CheckPlaneacionMensuals.ToList();
            foreach (var suc in sucursales)
            {
                foreach (var artbd in articulosbd)
                {
                    var art = _contextdb2.Articulos.Where(x => x.Codarticulo == artbd.Codarticulo).FirstOrDefault();
                    string codalmacen = suc.cod > 9 ? suc.cod.ToString() : "0" + suc.cod;
                    List<double> comprasart = new List<double>();

                    for (int i = 1; i <= 12 ; i++)
                    {
                        DateTime fecha = DateTime.Now.AddMonths((i * -1));
                        int mes = fecha.Month;
                        int año = fecha.Year;
                        DateTime fi = new DateTime(año, mes, 1);
                        int diasEnMes = DateTime.DaysInMonth(año, mes);
                        DateTime ff = new DateTime(año, mes, diasEnMes);

                        double temp = await _calculadora.getComprasDelPeriodo(fi, ff, art.Codarticulo, codalmacen);
                        comprasart.Add(temp);

                    }
                    DateTime hoy = DateTime.Today;
                    DateTime primerDia = new DateTime(hoy.Year, hoy.Month, 1);
                    var preciocompra = _contextdb2.Precioscompras.Where(x => x.Codarticulo == art.Codarticulo && x.Codproveedor == artbd.Codproveedor).FirstOrDefault();
                    var proveedor = _contextdb2.Proveedores.Where(x => x.Codproveedor == artbd.Codproveedor).FirstOrDefault();
                    var itprod = _contextdb2.ItProductos.Where(p => p.Rfc == proveedor.Nif20 && p.Codarticulo == art.Codarticulo).FirstOrDefault();
                    int multiplocompra = 1;
                    if (itprod != null) { multiplocompra = (int)itprod.Uds; }
                    var stock = _contextdb2.Moviments.Where(x => x.Codalmacenorigen == codalmacen && x.Codalmacendestino == "" && x.Tipo == "REG"
                    && x.Codarticulo == art.Codarticulo && x.Fecha.Value.Date == primerDia).FirstOrDefault();
                    double valorinventario = 0;
                    double precio = 0;

                    var itemimpuesto = _contextdb2.Impuestos.Where(p => p.Tipoiva == art.Impuestocompra).FirstOrDefault();
                    double ivaArt = (double)(itemimpuesto.Iva == null ? 16 : itemimpuesto.Iva);

                    if (preciocompra != null) { precio = (double)preciocompra.Pbruto; }
                    if (stock != null) { valorinventario = (double)stock.Unidades; }
                    ConsumoMensualInput itemInput = new ConsumoMensualInput();
                    itemInput.CodArticulo = art.Codarticulo;
                    itemInput.nombreprov = proveedor.Nomproveedor; 
                    itemInput.codProveedor = proveedor.Codproveedor;
                    itemInput.idSucursal = suc.cod;
                    itemInput.Ubicacion = suc.name;
                    itemInput.Referencia = art.Refproveedor;
                    itemInput.Descripcion = art.Descripcion;
                    itemInput.Medida = art.Unidadmedida;
                    itemInput.MultiploCompra = multiplocompra;
                    itemInput.Consumos = comprasart;
                    itemInput.StockFisico = valorinventario;
                    itemInput.precio = precio;
                    itemInput.tipoimpuesto = (int)art.Impuestocompra;
                    itemInput.iva = ivaArt; 
                    datosinput.Add(itemInput);
                }
            }

            request.Datos = datosinput;
            if (request == null || request.Parametros == null || request.Datos == null)
                return BadRequest("Faltan datos de entrada.");

            List<ResultadoPedido> resultados = new List<ResultadoPedido>();

            foreach (var item in request.Datos)
            {
                var resultado = _calculadora.Calcular(item, request.Parametros);
                resultados.Add(resultado);
            }

            List<ConsumoMensualGrupo> data = resultados
            .GroupBy(x => new { x.idSucursal, x.codProveedor })
            .Select(g => new ConsumoMensualGrupo
            {
                idSucursal = g.Key.idSucursal,
                codProveedor = g.Key.codProveedor
            })
            .ToList();

            foreach(var itemg in data)
            {
                int numerodepedidos = 1; 
                var segmentacion = parametros.segmentacionpedidos.Where(x=>x.codprov == itemg.codProveedor).FirstOrDefault();
                if (segmentacion != null) 
                {
                    numerodepedidos = segmentacion.division; 
                }

                var pedidocab = new PedidosMensualCab();
                pedidocab.DivisionPedidos = numerodepedidos; 
                pedidocab.Codproveedor = itemg.codProveedor; 
                pedidocab.Idsucursal = itemg.idSucursal;
                pedidocab.Fecha = DateTime.Now;
                pedidocab.Estatus = "POR ACEPTAR";

                var itembdaut = _dbpContext.PedidosMensualCabs.Where(x => x.Idsucursal == pedidocab.Idsucursal && x.Codproveedor == pedidocab.Codproveedor && x.Estatus == "AUTORIZADO").FirstOrDefault(); 
                if(itembdaut != null) { continue; }

                await _dbpContext.PedidosMensualCabs.AddAsync(pedidocab);
                await _dbpContext.SaveChangesAsync();

                var items_originales = resultados.Where(x => x.idSucursal == itemg.idSucursal && x.codProveedor == itemg.codProveedor).ToList();
                
                for (int i = 0; i <=  numerodepedidos; i++) 
                {
                    foreach (var input in items_originales)
                    {
                        var itembd = _dbpContext.PedidosMensualLins.Where(x => x.IdSucursal == input.idSucursal && x.CodProveedor == input.codProveedor && x.Codarticulo == input.codarticulo && x.Idcab == pedidocab.Id && x.NumpedidoLin == i && x.Estatus != "AUTORIZADO").FirstOrDefault();

                        if (itembd != null)
                        {
                            _dbpContext.PedidosMensualLins.Remove(itembd);
                            await _dbpContext.SaveChangesAsync();
                        }
                        int cajasdivididas = i > 0 ? (int)Math.Ceiling((double)input.PedidoSugerido / numerodepedidos) : input.PedidoSugerido;
                        var item = new PedidosMensualLin()
                        {
                            Codarticulo = input.codarticulo,
                            Ubicacion = input.Ubicacion,
                            Referencia = input.Referencia,
                            Descripcion = input.Descripcion,
                            ConsumoPromedio = input.ConsumoPromedio,
                            DesviacionEstandar = input.DesviacionEstandar,
                            NivelObjetivo = input.NivelObjetivo,
                            StockFisico = input.StockFisico,
                            PedidoSugerido = cajasdivididas,
                            CodProveedor = input.codProveedor,
                            IdSucursal = input.idSucursal,
                            Nombreprov = input.nombreprov,
                            Udscaja = input.udscaja,
                            Precio = input.precio,
                            Tipoimpuesto = input.tipoimpuesto,
                            Iva = input.iva,
                            Estatus = "POR ACEPTAR",
                            Fecha = DateTime.Now,
                            Idcab = pedidocab.Id,
                            NumpedidoLin = i
                        };
                        _dbpContext.PedidosMensualLins.Add(item);
                        await _dbpContext.SaveChangesAsync();
                    }
                }
            }

          return Ok();
        }


        [HttpPost]
        [Route("ConfirmarPedido")]
        public async Task<ActionResult> confirmarPedido([FromForm] int idp,[FromForm] int nump,[FromForm] DateTime fechaentrega)
        {
            try
            {

                var pedidodb = _dbpContext.PedidosMensualCabs.Where(x=>x.Id == idp).FirstOrDefault();
                var articulos = _dbpContext.PedidosMensualLins.Where(x => x.Idcab == pedidodb.Id && x.NumpedidoLin == nump).ToList();
                SqlConnection connection = (SqlConnection)_dbpContext.Database.GetDbConnection();
                connection.Open();

                SqlTransaction transaccion = connection.BeginTransaction();

                var remfront = _contextdb2.RemFronts.Where(x => x.Idfront == int.Parse(pedidodb.Idsucursal.ToString())).FirstOrDefault();
                var cajafront = _contextdb2.RemCajasfronts.Where(x => x.Cajafront == 1 && x.Idfront == int.Parse(pedidodb.Idsucursal.ToString())).FirstOrDefault();
                var codcliente = remfront.Codcliente;
                var transporte = _contextdb2.Transportes.Where(x => x.Fax == pedidodb.Idsucursal.ToString()).FirstOrDefault();
                int idtransporte = 0;
                if (transporte != null) { idtransporte = transporte.Codigo; }
                string numserie = cajafront.Cajamanager + "X";

                string codalmacen = "";

                if (pedidodb.Idsucursal < 10)
                {
                    codalmacen = "0" + pedidodb.Idsucursal;
                }
                else { codalmacen = pedidodb.Idsucursal.ToString(); }

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
                        string csupedido = numserie + "-" + numpedido;
                        double totalimpuestos = 0;
                        double totalpedido = 0;
                        foreach (var item in articulos)
                        {
                            totalpedido += (item.Udscaja.GetValueOrDefault() * item.PedidoSugerido * item.Precio);
                            totalimpuestos += ((item.Udscaja.GetValueOrDefault()*item.PedidoSugerido*item.Precio) * item.Iva) / 100;
                        }

                        // insertar pedcompracab
                        command = new SqlCommand("SP_INSERT_PEDIDO", connection, transaccion);
                        command.CommandType = CommandType.StoredProcedure;
                        // Parámetros del procedimiento almacenado
                        command.Parameters.AddWithValue("@PEDCAB_NUMSERIE", numserie);
                        command.Parameters.AddWithValue("@PEDCAB_NUMPEDIDO", numpedido);
                        command.Parameters.AddWithValue("@PEDCAB_CODPROVEEDOR", pedidodb.Codproveedor);
                        command.Parameters.AddWithValue("@PEDCAB_FECHA_PEDIDO", DateTime.Now);
                        command.Parameters.AddWithValue("@PEDCAB_FECHA_ENTREGA", fechaentrega);
                        command.Parameters.AddWithValue("@PEDCAB_TOTBRUTO", totalpedido);
                        command.Parameters.AddWithValue("@PEDCAB_TOTIMPUESTOS", totalimpuestos);
                        command.Parameters.AddWithValue("@PEDCAB_TOTNETO", totalpedido + totalimpuestos);
                        command.Parameters.AddWithValue("@PEDCAB_SUPEDIDO", supedido);
                        command.Parameters.AddWithValue("@TRANSPORTE", idtransporte);
                        command.ExecuteNonQuery();

                        // insertar pedcompralin
                        int numlinea = 0;
                        foreach (var art in articulos)
                        {
                            numlinea++;
                            var articulodb = _contextdb2.Articulos.Where(x => x.Codarticulo == art.Codarticulo).FirstOrDefault();
                            string referencia = articulodb.Refproveedor;
                            command = new SqlCommand("SP_INSERT_PEDIDOLIN", connection, transaccion);
                            command.CommandType = CommandType.StoredProcedure;
                            // Agregar parámetros
                            command.Parameters.AddWithValue("@PEDLIN_NUMSERIE", numserie);
                            command.Parameters.AddWithValue("@PEDLIN_NUMPEDIDO", numpedido);
                            command.Parameters.AddWithValue("@PEDLIN_NUMLINEA", numlinea);
                            command.Parameters.AddWithValue("@PEDLIN_CODARTICULO", art.Codarticulo);
                            command.Parameters.AddWithValue("@PEDLIN_REFERENCIA", referencia);
                            command.Parameters.AddWithValue("@PEDLIN_DESCRIPCION", articulodb.Descripcion);
                            command.Parameters.AddWithValue("@PEDLIN_CAJAS", art.PedidoSugerido);
                            command.Parameters.AddWithValue("@PEDLIN_UNIDADES", art.Udscaja.GetValueOrDefault());
                            command.Parameters.AddWithValue("@PEDLIN_UDSTOTALES", (art.Udscaja.GetValueOrDefault()*art.PedidoSugerido));
                            command.Parameters.AddWithValue("@PEDLIN_PRECIO", art.Precio);
                            command.Parameters.AddWithValue("@PEDLIN_TIPOIMPUESTO", art.Tipoimpuesto);
                            command.Parameters.AddWithValue("@PEDLIN_IVA", art.Iva);
                            command.Parameters.AddWithValue("@PEDLIN_IEPS", 0);
                            command.Parameters.AddWithValue("@PEDLIN_TOTAL", ((art.Udscaja.GetValueOrDefault() * art.PedidoSugerido)*art.Precio));
                            command.Parameters.AddWithValue("@PEDLIN_CODALMACEN", codalmacen);
                            command.Parameters.AddWithValue("@PEDLIN_SUPEDIDO", supedido);
                            command.Parameters.AddWithValue("@PEDLIN_FECHAENTREGA", fechaentrega);
                            command.ExecuteNonQuery();
                        }

                        // insertar pedcompratot
                        int numlineatot = 0;
                        var impuestos = articulos.Select(articulo => articulo.Tipoimpuesto).Distinct();
                        foreach (var impuesto in impuestos)
                        {
                            numlineatot++;
                            double totalbruto = 0;
                            var articulosimp = articulos.Where(x => x.Tipoimpuesto == impuesto);
                            double iva = 0;
                            foreach (var item in articulosimp)
                            {
                                totalbruto += ((item.Udscaja.GetValueOrDefault() * item.PedidoSugerido) * item.Precio);
                                iva = item.Iva;
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

                        var prov = _contextdb2.Proveedores.Where(x => x.Codproveedor == pedidodb.Codproveedor).FirstOrDefault();
                        var fpagoprov = _contextdb2.Fpagoproveedors.Where(x => x.Codproveedor == pedidodb.Codproveedor).FirstOrDefault();
                        command = new SqlCommand("SP_INSERT_TESORERIA", connection, transaccion);
                        command.CommandType = CommandType.StoredProcedure;

                        // Agregar parámetros al procedimiento almacenado
                        command.Parameters.AddWithValue("@TES_NUMSERIE", numserie);
                        command.Parameters.AddWithValue("@TES_NUMPEDIDO", numpedido);
                        command.Parameters.AddWithValue("@TES_CUENTA", prov.Codcontable);
                        command.Parameters.AddWithValue("@TES_CODPROV", prov.Codproveedor);
                        command.Parameters.AddWithValue("@TES_IMPORTE", totalpedido + totalimpuestos);
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


                        if (prov.Codproveedor == 5 || prov.Codproveedor == 1 || prov.Codproveedor == 10)
                        {
                            command = new SqlCommand("SP_INSERT_INCIDENCIA", connection, transaccion);

                            command.CommandType = CommandType.StoredProcedure;

                            // Agregar los parámetros
                            command.Parameters.AddWithValue("@FECHA", DateTime.Now.Date);
                            command.Parameters.AddWithValue("@CODCLIENTE", codcliente);
                            command.Parameters.AddWithValue("@SERIE", numserie);
                            command.Parameters.AddWithValue("@NUMPEDIDO", numpedido);
                            command.Parameters.AddWithValue("@FECHAENTREGA", fechaentrega.Date);
                            command.Parameters.AddWithValue("@CSUPEDIDO", csupedido);
                            command.Parameters.AddWithValue("@CODPROV", pedidodb.Codproveedor);
                            command.Parameters.AddWithValue("@COMENTARIOLIBRE", "");
                            command.Parameters.AddWithValue("@TOTALSINIVA", totalpedido);
                            command.Parameters.AddWithValue("@TOTALCONIVA", totalpedido + totalimpuestos);
                            command.Parameters.AddWithValue("@IDF", pedidodb.Idsucursal);

                            command.ExecuteNonQuery();

                            command = new SqlCommand("[dbo].[GET_IDINCIDENCIA]", connection, transaccion);
                            command.CommandType = CommandType.StoredProcedure;

                            object result2 = command.ExecuteScalar();
                            int idincidencia = Convert.ToInt32(result2);

                            numlinea = 0;
                            foreach (var art in articulos)
                            {
                                numlinea++;
                                var articulodb = _contextdb2.Articulos.Where(x => x.Codarticulo == art.Codarticulo).FirstOrDefault();
                                command = new SqlCommand("SP_INSERT_INCIDENCIA_LIN", connection, transaccion);

                                command.CommandType = CommandType.StoredProcedure;

                                // Agregar los parámetros
                                command.Parameters.AddWithValue("@IDINCIDENCIA", idincidencia);
                                command.Parameters.AddWithValue("@NUMLINEA", numlinea);
                                command.Parameters.AddWithValue("@CODART", art.Codarticulo);
                                command.Parameters.AddWithValue("@UNIDADES", art.PedidoSugerido);
                                command.Parameters.AddWithValue("@UNIDADES2", art.Udscaja.GetValueOrDefault());
                                command.Parameters.AddWithValue("@TOTALLINEA", (art.PedidoSugerido * art.Udscaja.GetValueOrDefault() * art.Precio));
                                command.Parameters.AddWithValue("@DESCRIPCIONART", articulodb.Descripcion);
                                command.Parameters.AddWithValue("@CODBARRAS", "");

                                command.ExecuteNonQuery();
                            }
                        }


                        await transaccion.CommitAsync();

                        foreach (var art in articulos) 
                        {
                            art.Estatus = "AUTORIZADO";
                            art.Numpedido = supedido;
                            art.HoraCargaIcg = DateTime.Now;
                            art.FechaEntrega = fechaentrega.Date; 
                            _dbpContext.PedidosMensualLins.Update(art); 
                            await _dbpContext.SaveChangesAsync();
                        }

                        var articulos_todos = _dbpContext.PedidosMensualLins.Where(x=>x.Idcab == pedidodb.Id && x.NumpedidoLin > 0).ToList();
                        Boolean pedido_completo = false;
                        if (articulos_todos.Count == articulos_todos.Where(x => x.Estatus == "AUTORIZADO").ToList().Count) 
                        {
                            pedido_completo = true; 
                        }

                        if (pedido_completo == true) 
                        {
                             pedidodb.Estatus = "AUTORIZADO";
                            _dbpContext.PedidosMensualCabs.Update(pedidodb); 
                            await _dbpContext.SaveChangesAsync();
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

    public class itemSucursal
    {
        public int cod { get; set; }
        public string name { get; set; }
    }

    public class DivisionPedidos
    {
        public int codprov { get; set; }
        public string nomprov { get; set; }
        public int division { get; set; }
    }

    public class SolicitudCalculo
    {
        public ParametrosCalculo Parametros { get; set; }
        public List<ConsumoMensualInput> Datos { get; set; }
    }

    public class ParametrosCalculo
    {
        public int LeadTimeDias { get; set; } = 7;
        public int PeriodoRevisionDias { get; set; } = 30;
        public double Z { get; set; } = 1.65;
        public int N { get; set; } = 4;
        public int MesObjetivo { get; set; }
        public Dictionary<string, double> AjustesEstacionales { get; set; }
        public List<DivisionPedidos> segmentacionpedidos {  get; set; }
    }

    public class ConsumoMensualInput
    {
        public int CodArticulo { get; set; }
        public int codProveedor { get; set; }
        public int idSucursal { get; set; }
        public string Ubicacion { get; set; }
        public string Referencia { get; set; }
        public string Descripcion { get; set; }
        public string Medida { get; set; }
        public int MultiploCompra { get; set; } = 1;
        public List<double> Consumos { get; set; }
        public double StockFisico { get; set; }
        public string nombreprov {  get; set; }
        public double precio { get; set; }  
        public int tipoimpuesto { get; set; }
        public double iva {  get; set; }
    }

    public class ResultadoPedido
    {
        public string Ubicacion { get; set; }
        public string Referencia { get; set; }
        public string Descripcion { get; set; }
        public double ConsumoPromedio { get; set; }
        public double DesviacionEstandar { get; set; }
        public double NivelObjetivo { get; set; }
        public double StockFisico { get; set; }
        public int PedidoSugerido { get; set; }
        public string nombreprov { get; set; }
        public int idSucursal { get; set; }
        public int codProveedor { get; set; }
        public double udscaja { get; set; }
        public double precio { get; set; } 
        public int tipoimpuesto { get; set; }
        public double iva { get; set; }
        public int codarticulo { get; set; }
        public int idcab {  get; set; }
        public int? numpedidolin {  get; set; }
        public int numpedido { get; set; }
        public DateTime? fechaentrega { get; set; }

    }

    public class ConsumoMensualGrupo
    {
        public int id {  get; set; }
        public int idSucursal { get; set; }
        public int codProveedor { get; set; }
        public DateTime fecha {  get; set; }
        public string estatus { get; set; }
        public int division { get; set; }   
        public List<PedidosMensualLin> Items { get; set; }
    }

    public class CalculadoraPedidos
    {
        public string connectionStringBD2 = string.Empty;
        protected DBPContext _dbpContext;
        protected BD2Context _contextdb2;

        public CalculadoraPedidos(DBPContext dbpContext,BD2Context contextdb2) 
        {
            _dbpContext = dbpContext;
            _contextdb2 = contextdb2;
            connectionStringBD2 = contextdb2.Database.GetConnectionString();
        }

        public ResultadoPedido Calcular(ConsumoMensualInput input, ParametrosCalculo parametros)
        {
            var consumos = input.Consumos.Take(parametros.N).ToList();
            double promedio = consumos.Average();
            double desvEst = Math.Sqrt(consumos.Select(x => Math.Pow(x - promedio, 2)).Sum() / parametros.N);

            double ajuste = parametros.AjustesEstacionales?.GetValueOrDefault(input.Referencia) ?? 1.0;

            double leadReview = parametros.LeadTimeDias + parametros.PeriodoRevisionDias;
            double nivelObjetivo = (promedio * ajuste / 30.0) * leadReview
                                   + parametros.Z * desvEst * Math.Sqrt(leadReview / 30.0);

            double pedido = Math.Max(0, nivelObjetivo - input.StockFisico);
            int pedidoSugerido = (int)Math.Ceiling(pedido / input.MultiploCompra) * input.MultiploCompra;

            return new ResultadoPedido
            {
                Ubicacion = input.Ubicacion,
                Referencia = input.Referencia,
                Descripcion = input.Descripcion,
                ConsumoPromedio = promedio,
                DesviacionEstandar = desvEst,
                NivelObjetivo = nivelObjetivo,
                StockFisico = input.StockFisico,
                PedidoSugerido = pedidoSugerido,
                codProveedor = input.codProveedor,
                idSucursal = input.idSucursal,
                nombreprov = input.nombreprov,
                udscaja = input.MultiploCompra,
                precio = input.precio,
                tipoimpuesto = input.tipoimpuesto,
                iva = input.iva,
                codarticulo = input.CodArticulo
            };
        }

        public async Task<double> getComprasDelPeriodo(DateTime fi, DateTime ff, int codarticulo, string codalmacen) 
        {
            double Compras = 0; double Consumos = 0;
            var connectionString = _dbpContext.Database.GetConnectionString();
            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand("SP_GET_REMINISIONES_ART", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("@FI", SqlDbType.DateTime).Value = fi;
                    command.Parameters.Add("@FF", SqlDbType.DateTime).Value = ff;
                    command.Parameters.Add("@CODARTICULO", SqlDbType.Int).Value = codarticulo;
                    command.Parameters.Add("@CODALMACEN", SqlDbType.NVarChar).Value = codalmacen;

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Compras = Compras + (double)Convert.ToDecimal(reader["COMPRAS"]);
                            Consumos = Consumos + (double)Convert.ToDecimal(reader["CONSUMOS"]);
                        }
                    }
                }
            }
            return Compras;
        }
        public async Task<List<itemSucursal>> GetSucursales()
        {
            try
            {

                string query = @"
       SELECT RF.IDFRONT AS cod, RF.TITULO AS name
FROM ALMACEN ALM WITH(NOLOCK)
INNER JOIN REM_CAJASFRONT RCF WITH(NOLOCK) ON ALM.CODALMACEN COLLATE Latin1_General_CS_AI = RCF.CODALMVENTAS
INNER JOIN SERIESCAMPOSLIBRES SCL WITH(NOLOCK) ON RCF.SERIETIQUETS COLLATE Latin1_General_CS_AI = SCL.SERIE
INNER JOIN REM_FRONTS RF ON RF.IDFRONT = RCF.IDFRONT 
WHERE (ALM.NOTAS LIKE N'RW') AND (RCF.CAJAFRONT = 1)";

                List<itemSucursal> sucursales = new List<itemSucursal>();

                using (SqlConnection connection = new SqlConnection(connectionStringBD2))
                {
                    SqlCommand command = new SqlCommand(query, connection);

                    // Abrir la conexión
                    connection.Open();

                    // Ejecutar el comando y obtener los datos
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Crear una tabla para almacenar los datos
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);

                        // Imprimir los datos (para prueba)
                        foreach (DataRow row in dataTable.Rows)
                        {
                            sucursales.Add(new itemSucursal(){ cod = (int)row[0], name = (string)row[1] });
                        }
                    }

                }

                return sucursales;
            }
            catch (Exception ex)
            {
                return new List<itemSucursal>();
            }
        }
    }

}
