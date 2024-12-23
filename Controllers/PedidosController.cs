using API_PEDIDOS.funciones;
using API_PEDIDOS.ModelsDB2;
using API_PEDIDOS.ModelsDBP;
using Humanizer.Localisation;
using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using NuGet.Packaging.Signing;
using NuGet.Protocol;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart.ChartEx;
using OfficeOpenXml.Style;
using System;
using System.Collections.Specialized;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography.X509Certificates;
using System.Transactions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Text.Json;

namespace API_PEDIDOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidosController : ControllerBase
    {
        private readonly ILogger<CatalogosController> _logger;
        protected BD2Context _contextdb2;
        protected DBPContext _dbpContext;

        public PedidosController(ILogger<CatalogosController> logger, BD2Context db2c, DBPContext dbpc)
        {
            _logger = logger;
            _contextdb2 = db2c;
            _dbpContext = dbpc;
        }

        [HttpGet]
        [Route("getinventarios/{sucursal}/{articulo}")]
        public async Task<ActionResult> GetInventario(int sucursal, int articulo, [FromQuery] string fechainicio, [FromQuery] string fechafin)
        {
            try
            {
                List<PinventarioModel> inventarios = new List<PinventarioModel>();

                // Crear y configurar la conexión y el comando para el procedimiento almacenado
                using (SqlConnection connection = (SqlConnection)_dbpContext.Database.GetDbConnection())
                {
                    using (SqlCommand command = new SqlCommand("SP_GET_INVENTARIO", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        string codalm = "";
                        if (sucursal < 10)
                        {
                            codalm = "0" + sucursal;
                        }
                        else { codalm = sucursal.ToString(); }
                        // Añadir parámetros al comando
                        command.Parameters.Add("@sucursal", SqlDbType.NVarChar, 5).Value = codalm;
                        command.Parameters.Add("@articulo", SqlDbType.Int).Value = articulo;
                        command.Parameters.Add("@FI", SqlDbType.NVarChar, 255).Value = fechainicio;
                        command.Parameters.Add("@FF", SqlDbType.NVarChar, 255).Value = fechafin;

                        // Abrir la conexión
                        connection.Open();

                        // Ejecutar el comando y leer los resultados
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DateTime fecha = reader.GetDateTime(0);
                                double unidades = reader.GetDouble(1);

                                inventarios.Add(new PinventarioModel()
                                {
                                    fecha = fecha,
                                    unidades = unidades,
                                });
                            }
                        }
                    }

                }

                if (inventarios.Count < 7)
                {
                    int faltante = 7 - inventarios.Count;
                    for (int i = 0; i < faltante; i++)
                    {
                        inventarios.Add(new PinventarioModel()
                        {
                            fecha = DateTime.Now,
                            unidades = 0,
                        });
                    }
                }

                return StatusCode(200, inventarios);
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
        [Route("getPedidosHoy/{idu}")]
        public async Task<ActionResult> GetPedidos(int idu)
        {
            try
            {
                _dbpContext.ValidacionPedidos.Add(new ValidacionPedido() { Status = true, Idu =idu });
                await _dbpContext.SaveChangesAsync();
                var asignaciones = _dbpContext.AsignacionProvs.Where(x => x.Idu == idu).ToList();
              
                var parametros = _dbpContext.Parametros.FirstOrDefault();
                dynamic obj = JsonConvert.DeserializeObject<dynamic>(parametros.Jdata);

                List<Pedido> rangopedidosdel = new List<Pedido>();
                var delpedidos = _dbpContext.Pedidos.Where(x => x.Fecha.Value.Date == DateTime.Now.Date && (x.Estatus == "POR ACEPTAR" || x.Estatus == "INCOMPLETO") && x.Temporal != true).ToList();
                foreach (var item in delpedidos)
                {
                    if (asignaciones.Any(x => x.Idprov == item.Proveedor && x.Idsuc == int.Parse(item.Sucursal)))
                    {
                        rangopedidosdel.Add(item);
                    }
                }

                foreach (var item in rangopedidosdel)
                {
                    var modificaciones = _dbpContext.Modificaciones.Where(x => x.IdPedido == item.Id).ToList();
                    if (modificaciones.Count > 0)
                    {
                        _dbpContext.Modificaciones.RemoveRange(modificaciones);
                        await _dbpContext.SaveChangesAsync();
                    }

                }

                _dbpContext.RemoveRange(rangopedidosdel);
                await _dbpContext.SaveChangesAsync();

                SqlConnection conn = (SqlConnection)_dbpContext.Database.GetDbConnection();
                conn.Open();
                List<Pedidos> pedidos = new List<Pedidos>();
                List<Calendario> calendarioshoy = new List<Calendario>();
                DateTime fechaHoy = DateTime.Now;
                DateTime fechaentrega = DateTime.Now;
                DayOfWeek diaSemana = fechaHoy.DayOfWeek;
                int numdia = 0;
                switch (diaSemana)
                {
                    case DayOfWeek.Monday:
                        numdia = 1;
                        break;
                    case DayOfWeek.Tuesday:
                        numdia = 2;
                        break;
                    case DayOfWeek.Wednesday:
                        numdia = 3;
                        break;
                    case DayOfWeek.Thursday:
                        numdia = 4;
                        break;
                    case DayOfWeek.Friday:
                        numdia = 5;
                        break;
                    case DayOfWeek.Saturday:
                        numdia = 6;
                        break;
                    case DayOfWeek.Sunday:
                        numdia = 0;
                        break;
                    default:
                        break;
                }
                var calendarios = _dbpContext.Calendarios.Where(x => x.Temporal != true).ToList();
                foreach (var item in calendarios)
                {
                    int[][] array = JsonConvert.DeserializeObject<int[][]>(item.Jdata);
                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i][numdia] == 1)
                        {
                            if (asignaciones.Any(x => x.Idprov == item.Codproveedor && x.Idsuc == item.Codsucursal)) 
                            {
                                calendarioshoy.Add(item);
                            }
                        }
                    }
                }

                DateTime[] fechas = new DateTime[7];
                DateTime tempdt = fechaHoy.AddDays(-numdia);
                for (int i = 0; i < 7; i++)
                {
                    fechas[i] = tempdt;
                    tempdt = tempdt.AddDays(1);
                }
               // fechas[0] = fechas[0].AddDays(7); 

                foreach (var item in calendarioshoy)
                {
                    Boolean articulosdiferentes = false;
                    var haypedido = _dbpContext.Pedidos.Where(x => x.Fecha.Value.Date == DateTime.Now.Date && x.Proveedor == item.Codproveedor && x.Sucursal == item.Codsucursal.ToString() && x.Temporal != true).ToList();
             

                    Boolean generarpedido = true;

                    foreach (var itemp in haypedido)
                    {
                        if (itemp.Idcal == item.Id)
                        {
                            generarpedido = false;
                        }
                    }

                    if (haypedido.Count == 0 || generarpedido) 
                    {
                        Boolean requierecartones = false; 
                        double totalpedido = 0;
                        string nombresucursal = "";
                        string nombreproveedor = "";
                        int status = 1;
                        string rfcprov = "";
                        var prov = _contextdb2.Proveedores.Where(p => p.Codproveedor == item.Codproveedor).FirstOrDefault();
                        rfcprov = prov.Nif20;
                        var itemproveedor = _contextdb2.Proveedores.Where(p => p.Codproveedor == item.Codproveedor).FirstOrDefault();
                        nombreproveedor = itemproveedor.Nomproveedor;

                        var itemsucursal = _contextdb2.RemFronts.Where(s => s.Idfront == item.Codsucursal).FirstOrDefault();
                        nombresucursal = itemsucursal.Titulo;

                        Boolean inventarioteorico = false;
                        var reginvt = _dbpContext.InventarioTeoricos.Where(x => x.Idfront == item.Codsucursal).FirstOrDefault();
                        if (reginvt != null)
                        {
                            var regprovinvt = _dbpContext.InvTeoricoProveedores.Where(x => x.Codprov == item.Codproveedor).FirstOrDefault(); 
                            if(regprovinvt != null) 
                            {
                                inventarioteorico = true;
                            }
                            
                        }

                        int[][] array = JsonConvert.DeserializeObject<int[][]>(item.Jdata);
                        double consumopromedio = 0;

                        List<articuloModel> articulosl = new List<articuloModel>();
                        var articulosdb = _dbpContext.ArticulosProveedors.Where(x => x.Idcalendario == item.Id).ToList();
                        foreach (var artdb in articulosdb)
                        {
                            var tempq = from art in _contextdb2.Articulos1
                                        join artcl in _contextdb2.Articuloscamposlibres on art.Codarticulo equals artcl.Codarticulo
                                        into gj
                                        from subartcl in gj.DefaultIfEmpty()
                                        join prec in _contextdb2.Precioscompras on art.Codarticulo equals prec.Codarticulo
                                        into gj2
                                        from subprec in gj2.DefaultIfEmpty()
                                        where subartcl != null && subartcl.Codarticulo == artdb.Codarticulo && subprec.Codproveedor == item.Codproveedor
                                        select new
                                        {
                                            cod = art.Codarticulo,
                                            descripcion = art.Descripcion,
                                            precio = subprec.Pbruto,
                                            referencia = art.Referenciasprovs,
                                            tipoimpuesto = art.Impuestocompra
                                        };
                            var tempart = tempq.FirstOrDefault();
                            articulosl.Add(new articuloModel()
                            {
                                cod = tempart.cod,
                                descripcion = tempart.descripcion,
                                precio = (double)tempart.precio,
                                referencia = tempart.referencia,
                                tipoimpuesto = (int)tempart.tipoimpuesto
                            });

                        }
                        

                        var query = from art in _contextdb2.Articulos1
                                    join artcl in _contextdb2.Articuloscamposlibres on art.Codarticulo equals artcl.Codarticulo
                                    into gj
                                    from subartcl in gj.DefaultIfEmpty()
                                    join prec in _contextdb2.Precioscompras on art.Codarticulo equals prec.Codarticulo
                                    into gj2
                                    from subprec in gj2.DefaultIfEmpty()
                                    where subartcl != null && subartcl.Planeacion == "T" && subprec.Codproveedor == item.Codproveedor
                                    select new articuloModel()
                                    {
                                        cod = art.Codarticulo,
                                        descripcion = art.Descripcion,
                                        precio = (double)subprec.Pbruto,
                                        referencia = art.Referenciasprovs,
                                        tipoimpuesto = (int)art.Impuestocompra
                                    };


                        int count = articulosl.Count;

                        var articulos = articulosl.Count > 0 ? articulosl : query.ToList();

                        List<ArticuloPedido> articulospedido = new List<ArticuloPedido>();
                        int numlinea = 0;
                        foreach (var art in articulos)
                        {
                            Boolean esretornable = false;
                            if (_dbpContext.Retornables.Where(x => x.Codart == art.cod).ToList().Count() > 0)
                            {
                                esretornable = true;
                                requierecartones = true; 
                            }

                            fechaentrega = DateTime.Now;
                            numlinea++;
                            List<ConsumoModel> consumos = new List<ConsumoModel>();
                            consumos.Clear();

                            // Crear comando para ejecutar el procedimiento almacenado
                            using (SqlCommand command = new SqlCommand("SP_Consumo_Promedio", conn))
                            {
                                command.CommandType = CommandType.StoredProcedure;
                                string codalm = "";
                                if (item.Codsucursal < 10)
                                {
                                    codalm = "0" + item.Codsucursal;
                                }
                                else { codalm = item.Codsucursal.ToString(); }
                                // Agregar parámetros al procedimiento almacenado



                                int parametrosemanas = (int)obj.pedido.diasconprom;

                                command.Parameters.Add("@sucursal", SqlDbType.NVarChar).Value = codalm;
                                command.Parameters.Add("@articulo", SqlDbType.Int).Value = art.cod;
                                command.Parameters.Add("@semanas", SqlDbType.Int).Value = parametrosemanas;

                                try
                                {
                                    // Ejecutar el procedimiento almacenado
                                    SqlDataReader reader = command.ExecuteReader();

                                    while (reader.Read())
                                    {
                                        consumos.Add(new ConsumoModel
                                        {
                                            dia = int.Parse(reader["DIA"].ToString()),
                                            consumo = double.Parse(reader["CONSUMO"].ToString())
                                        });
                                    }

                                    reader.Close();
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Error al ejecutar el procedimiento almacenado: " + ex.Message);
                                }
                            }

                            if (consumos.Count < 7) 
                            {
                                int iteraciones = 7 - consumos.Count;
                                for (int z = 0; z < iteraciones; z++) 
                                {
                                    consumos.Add(new ConsumoModel() 
                                    {
                                        dia = 0,
                                        consumo = 0
                                    });
                                }
                            }

                            consumopromedio = consumos[numdia].consumo;

                            double mayorconsumo = consumos.OrderByDescending(c => c.consumo).First().consumo;

                            double factorstock = (double)obj.pedido.factorstock;

                            double stockSeguridad = mayorconsumo * factorstock;

                            double consumopedido = 0;

                            int[] arraycal = new int[array[0].Length];
                            DiasEspecialesSucursal[] diasespeciales = { null, null, null, null, null, null, null };


                            for (int i = 0; i < array.Length; i++)
                            {
                                if (array[i][numdia] == 1)
                                {
                                    for (int j = 0; j < array[i].Length; j++)
                                    {

                                        if (array[i][j] == 1 || array[i][j] == 2 || array[i][j] == 3)
                                        {
                                            arraycal = array[i];
                                            var diaespecialsuc = _dbpContext.DiasEspecialesSucursals.ToList().Where(d => d.Fecha.Value.ToString("yyyy-MM-dd") == fechas[j].ToString("yyyy-MM-dd") && d.Sucursal == item.Codsucursal).FirstOrDefault();
                                            var diaespecial = _dbpContext.DiasEspeciales.ToList().Where(d => d.Fecha.ToString("yyyy-MM-dd") == fechas[j].ToString("yyyy-MM-dd")).FirstOrDefault();
                                            if (diaespecial == null && diaespecialsuc == null)
                                            {
                                                consumopedido += consumos[j].consumo;
                                            }
                                            else
                                            {
                                                if (diaespecialsuc == null)
                                                {
                                                    diasespeciales[j] = new DiasEspecialesSucursal()
                                                    {
                                                        Id = diaespecial.Id,
                                                        Dia = diaespecial.Dia,
                                                        Semana = diaespecial.Semana,
                                                        Fecha = diaespecial.Fecha,
                                                        Descripcion = diaespecial.Descripcion,
                                                        FactorConsumo = diaespecial.FactorConsumo,
                                                        Sucursal = 0
                                                    };
                                                    consumopedido += (consumos[j].consumo * diaespecial.FactorConsumo);
                                                }
                                                else
                                                {
                                                    int[] articulosdiesp = JsonConvert.DeserializeObject<int[]>(diaespecialsuc.Articulos);
                                                    if (articulosdiesp.Contains(art.cod))
                                                    {
                                                        diasespeciales[j] = diaespecialsuc;
                                                        double factor = (diaespecialsuc.FactorConsumo ?? 1.5);
                                                        consumopedido += (consumos[j].consumo * factor);
                                                    }
                                                    else 
                                                    {
                                                        if (diaespecial != null)
                                                        {
                                                            diasespeciales[j] = new DiasEspecialesSucursal()
                                                            {
                                                                Id = diaespecial.Id,
                                                                Dia = diaespecial.Dia,
                                                                Semana = diaespecial.Semana,
                                                                Fecha = diaespecial.Fecha,
                                                                Descripcion = diaespecial.Descripcion,
                                                                FactorConsumo = diaespecial.FactorConsumo,
                                                                Sucursal = 0
                                                            };
                                                            consumopedido += (consumos[j].consumo * diaespecial.FactorConsumo);
                                                        }
                                                        else 
                                                        {
                                                            consumopedido += consumos[j].consumo;
                                                        }
                                                    }
                                                  
                                                }
                                                
                                            }

                                        }
                                    }
                                }

                            }

                            int countday = numdia;
                            for (int i = 0; i < 7; i++)
                            {
                                countday++;
                                fechaentrega = fechaentrega.AddDays(1);
                                if (countday == 7)
                                {
                                    countday = 0;
                                }

                                if (arraycal[countday] == 3)
                                {
                                    break;
                                }
                            }


                            List<PinventarioModel> inventarios = new List<PinventarioModel>();
                            inventarios.Clear();

                            if (inventarioteorico && DateTime.Now.Date != new DateTime(2024, 12, 26).Date && DateTime.Now.Date != new DateTime(2025, 1, 2).Date)
                            {
                                using (SqlCommand command = new SqlCommand("SPS_GET_DIFERENCIA_LIN", conn))
                                {
                                    command.CommandType = CommandType.StoredProcedure;
                                    string codalm = "";
                                    if (item.Codsucursal < 10)
                                    {
                                        codalm = "0" + item.Codsucursal;
                                    }
                                    else { codalm = item.Codsucursal.ToString(); }
                                    // Añadir parámetros al comando
                                    command.Parameters.Add("@FECHA", System.Data.SqlDbType.VarChar, 10).Value = DateTime.Now.ToString("dd/MM/yyyy");
                                    command.Parameters.Add("@CODALM", System.Data.SqlDbType.NVarChar, 10).Value = codalm;
                                    command.Parameters.Add("@CODART", System.Data.SqlDbType.Int).Value = art.cod;
                                    command.CommandTimeout = 120;

                                    // Ejecutar el comando y leer los resultados
                                    using (SqlDataReader reader = command.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            DateTime fecha = DateTime.Now;
                                            double unidades = (double)reader["INVFORMULA"];
                                            inventarios.Add(new PinventarioModel()
                                            {
                                                fecha = fecha,
                                                unidades = unidades,
                                            });
                                        }
                                    }
                                }
                            }
                            else
                            {

                                using (SqlCommand command = new SqlCommand("SP_GET_INVENTARIO", conn))
                                {
                                    command.CommandType = CommandType.StoredProcedure;
                                    string codalm = "";
                                    if (item.Codsucursal < 10)
                                    {
                                        codalm = "0" + item.Codsucursal;
                                    }
                                    else { codalm = item.Codsucursal.ToString(); }
                                    // Añadir parámetros al comando
                                    command.Parameters.Add("@sucursal", SqlDbType.NVarChar, 5).Value = codalm;
                                    command.Parameters.Add("@articulo", SqlDbType.Int).Value = art.cod;
                                    command.Parameters.Add("@FI", SqlDbType.NVarChar, 255).Value = DateTime.Now.ToString("yyyy-MM-dd");
                                    command.Parameters.Add("@FF", SqlDbType.NVarChar, 255).Value = DateTime.Now.ToString("yyyy-MM-dd");

                                    // Ejecutar el comando y leer los resultados
                                    using (SqlDataReader reader = command.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            DateTime fecha = (DateTime)reader["FECHA"];
                                            double unidades = reader.GetDouble(1);

                                            inventarios.Add(new PinventarioModel()
                                            {
                                                fecha = fecha,
                                                unidades = unidades,
                                            });
                                        }
                                    }
                                }

                            }

                            double inventario = 0;
                            Boolean hayinventario = false;
                            if (inventarios.Count > 0) { inventario = inventarios[0].unidades; hayinventario = true; } else { status = 2; }
                            double proyeccion = 0; 
                            bool tieneudspendientes = false;
                            double unidadespendientes = 0; 
                            if (item.Especial == true)
                            {
                                string fechastr = DateTime.Now.ToString("yyyy-MM-dd");
                                var pedidosentrega = _dbpContext.Pedidos.Where(x => x.Proveedor == item.Codproveedor && x.Sucursal == item.Codsucursal.ToString()
                                && x.Jdata.Contains("\"fechaEntrega\":\""+fechastr) && x.Estatus == "AUTORIZADO").ToList();

                                foreach (var pe in pedidosentrega) 
                                {
                                    Pedidos p = JsonConvert.DeserializeObject<Pedidos>(pe.Jdata);
                                    var temp = p.articulos.Where(x => x.codArticulo == art.cod).FirstOrDefault();
                                    if (temp != null)
                                    {
                                        tieneudspendientes = true;
                                        unidadespendientes = temp.unidadestotales; 
                                        proyeccion = (consumopedido + stockSeguridad - inventario) - temp.unidadestotales;
                                    }
                                    else { proyeccion = (consumopedido + stockSeguridad - inventario);  } 
                                }

                                if (pedidosentrega.Count == 0) 
                                {
                                    proyeccion = (consumopedido + stockSeguridad - inventario); 
                                }

                            }
                            else 
                            {
                                proyeccion = (consumopedido + stockSeguridad - inventario);
                            }
                            int iva = 0;
                            var itprod = _contextdb2.ItProductos.Where(p => p.Rfc == rfcprov && p.Codarticulo == art.cod).FirstOrDefault();
                            Boolean tienemultiplo = itprod == null ? false : true;
                            if (!tienemultiplo) { status = 2; }
                            double unidadescaja = itprod == null ? 1 : (double)itprod.Uds;
                            int cajas = 0;
                            if (proyeccion % unidadescaja == 0)
                            {
                                cajas = (int)(proyeccion / unidadescaja);
                            }
                            else
                            {
                                double resultado = proyeccion / unidadescaja;
                                cajas = (int)Math.Floor(resultado) + 1;
                            }

                            double unidades_totales = cajas * unidadescaja;
                            double total_linea = (double)(unidades_totales * art.precio);
                            totalpedido += total_linea;
                            var itemimpuesto = _contextdb2.Impuestos.Where(p => p.Tipoiva == art.tipoimpuesto).FirstOrDefault();
                            double ivaArt = (double)(itemimpuesto.Iva == null ? 16 : itemimpuesto.Iva);


                            var regalmacenaje = _dbpContext.Almacenajes.Where(x => x.Idsucursal == item.Codsucursal && x.Codarticulo == art.cod).FirstOrDefault();
                            Boolean tienelimitealmacen = false;
                            double capacidadalm = 0;
                            double unidadesentrega = 0;
                            if (regalmacenaje != null)
                            {
                                capacidadalm = regalmacenaje.Capacidad;
                                string fechastr = DateTime.Now.ToString("yyyy-MM-dd");
                                var pedidosentrega = _dbpContext.Pedidos.Where(x => x.Proveedor == item.Codproveedor && x.Sucursal == item.Codsucursal.ToString()
                                && x.Jdata.Contains("\"fechaEntrega\":\"" + fechastr) && x.Estatus == "AUTORIZADO").ToList();

                                foreach (var pe in pedidosentrega)
                                {
                                    Pedidos p = JsonConvert.DeserializeObject<Pedidos>(pe.Jdata);
                                    var temp = p.articulos.Where(x => x.codArticulo == art.cod).FirstOrDefault();
                                    if (temp != null)
                                    {
                                        unidadesentrega = unidadesentrega + temp.unidadestotales;
                                    }
                                }

                                tienelimitealmacen = true;

                            }

                            articulospedido.Add(new ArticuloPedido()
                            {
                                codArticulo = art.cod,
                                nombre = art.descripcion,
                                inventariohoy = inventario,
                                precio = art.precio,
                                numlinea = numlinea,
                                cajas = cajas,
                                unidadescaja = unidadescaja,
                                unidadestotales = unidades_totales,
                                tipoImpuesto = (int)art.tipoimpuesto,
                                iva = ivaArt,
                                total_linea = total_linea,
                                codigoAlmacen = item.Codsucursal.ToString(),
                                tienemultiplo = tienemultiplo,
                                hayinventario = hayinventario,
                                consumospromedios = consumos,
                                consumomayor = mayorconsumo,
                                factorseguridad = 1.5,
                                arraycalendario = arraycal,
                                diasespeciales = diasespeciales,
                                calendarioespecial = tieneudspendientes,
                                unidadesextra = unidadespendientes,
                                esretornable = esretornable,
                                tienelimitealmacen = tienelimitealmacen,
                                capacidadalmfinal = (capacidadalm - unidadesentrega),
                                invformulado = inventarioteorico
                            });
                        }

                        // validar si requiere cartones 
                        double cartones = 0;
                        Boolean cartonescapturados = false; 
                        if (requierecartones) 
                        {
                            List<PinventarioModel> invcartones = new List<PinventarioModel>(); 

                            if (inventarioteorico && DateTime.Now.Date != new DateTime(2024, 12, 26).Date && DateTime.Now.Date != new DateTime(2025, 1, 2).Date)
                            {
                                using (SqlCommand command = new SqlCommand("SPS_GET_DIFERENCIA_LIN", conn))
                                {
                                    command.CommandType = CommandType.StoredProcedure;
                                    string codalm = "";
                                    if (item.Codsucursal < 10)
                                    {
                                        codalm = "0" + item.Codsucursal;
                                    }
                                    else { codalm = item.Codsucursal.ToString(); }
                                    // Añadir parámetros al comando
                                    command.Parameters.Add("@FECHA", System.Data.SqlDbType.VarChar, 10).Value = DateTime.Now.ToString("dd/MM/yyyy");
                                    command.Parameters.Add("@CODALM", System.Data.SqlDbType.NVarChar, 10).Value = codalm;
                                    command.Parameters.Add("@CODART", System.Data.SqlDbType.Int).Value = 10277;
                                    command.CommandTimeout = 120;

                                    // Ejecutar el comando y leer los resultados
                                    using (SqlDataReader reader = command.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            DateTime fecha = DateTime.Now;
                                            double unidades = (double)reader["INVFORMULA"];
                                            invcartones.Add(new PinventarioModel()
                                            {
                                                fecha = fecha,
                                                unidades = unidades,
                                            });
                                        }
                                    }
                                }
                            }
                            else
                            {

                                using (SqlCommand command = new SqlCommand("SP_GET_INVENTARIO", conn))
                                {
                                    command.CommandType = CommandType.StoredProcedure;
                                    string codalm = "";
                                    if (item.Codsucursal < 10)
                                    {
                                        codalm = "0" + item.Codsucursal;
                                    }
                                    else { codalm = item.Codsucursal.ToString(); }
                                    // Añadir parámetros al comando
                                    command.Parameters.Add("@sucursal", SqlDbType.NVarChar, 5).Value = codalm;
                                    command.Parameters.Add("@articulo", SqlDbType.Int).Value = 10277;
                                    command.Parameters.Add("@FI", SqlDbType.NVarChar, 255).Value = DateTime.Now.ToString("yyyy-MM-dd");
                                    command.Parameters.Add("@FF", SqlDbType.NVarChar, 255).Value = DateTime.Now.ToString("yyyy-MM-dd");

                                    // Ejecutar el comando y leer los resultados
                                    using (SqlDataReader reader = command.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            DateTime fecha = (DateTime)reader["FECHA"];
                                            double unidades = reader.GetDouble(1);

                                            invcartones.Add(new PinventarioModel()
                                            {
                                                fecha = fecha,
                                                unidades = unidades,
                                            });
                                        }
                                    }
                                }

                            }

                            if (invcartones.Count > 0) 
                            {
                                cartonescapturados = true;
                                cartones = invcartones[0].unidades; 
                            }
                        }

                        Boolean tienedescuento = false;
                        var regdesc = _dbpContext.Descuentos.Where(x => x.Codprov == item.Codproveedor).FirstOrDefault();

                        if (regdesc != null) { tienedescuento = true; }

                        pedidos.Add(new Pedidos()
                        {
                            idSucursal = item.Codsucursal.ToString()
                           ,
                            codProveedor = item.Codproveedor,
                            total = totalpedido,
                            fechaEntrega = fechaentrega,
                            articulos = articulospedido,
                            nombreproveedor = nombreproveedor,
                            nombresucursal = nombresucursal,
                            status = status,
                            rfc = rfcprov,
                            cartones= cartones,
                            tieneretornables = requierecartones,
                            capturacartones = cartonescapturados,
                            tienedescuento = tienedescuento,
                            cantidaddescuento = 0
                        });

                        string tempjdata = JsonConvert.SerializeObject(pedidos.Last());
                        var temppedido = _dbpContext.Pedidos.Where(p => p.Sucursal == item.Codsucursal.ToString() && p.Proveedor == item.Codproveedor && p.Jdata == tempjdata && p.Fecha.Value.Date == DateTime.Now.Date).FirstOrDefault();
                        if (temppedido == null)
                        {
                            await _dbpContext.Pedidos.AddAsync(new Pedido()
                            {
                                Sucursal = item.Codsucursal.ToString(),
                                Proveedor = item.Codproveedor,
                                Jdata = tempjdata,
                                Estatus = status == 1 ? "POR ACEPTAR" : "INCOMPLETO",
                                Fecha = DateTime.Now,
                                Numpedido = "",
                                Idcal = item.Id,
                                Temporal = false
                            });
                            await _dbpContext.SaveChangesAsync();
                        }
                        else
                        {
                            pedidos.RemoveAt(pedidos.Count - 1);
                        }

                    }

                }

                var estatuspedido = _dbpContext.ValidacionPedidos.Where(x => x.Idu == idu).FirstOrDefault(); 
                 if(estatuspedido != null) 
                {
                    _dbpContext.ValidacionPedidos.Remove(estatuspedido); 
                    await _dbpContext.SaveChangesAsync();
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
        [Route("GuardarMedidaUds")]
        public async Task<ActionResult> saveumedida([FromBody] itproductoModel model)
        {
            try
            {
                await _contextdb2.ItProductos.AddAsync(new ModelsDB2.ItProducto()
                {
                    Rfc = model.rfc,
                    Codarticulo = model.codarticulo,
                    Umedida = model.umedida,
                    Uds = model.uds,
                    NoIdentificacion = model.rfc + "-" + model.codarticulo + "-0"

                });
                await _contextdb2.SaveChangesAsync();
                return StatusCode(StatusCodes.Status200OK);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.ToString() });
            }


        }


        [HttpGet]
        [Route("GetMedidasUds")]
        public async Task<ActionResult> getumedidas()
        {
            try
            {
                var query = from prov in _contextdb2.Proveedores
                            join provcl in _contextdb2.Proveedorescamposlibres
                            on prov.Codproveedor equals provcl.Codproveedor into gj
                            from subprov in gj.DefaultIfEmpty()
                            where subprov != null && subprov.Planeacion == "T"
                            select new
                            {
                                codproveedor = prov.Codproveedor,
                                nombre = prov.Nomproveedor,
                                rfc = prov.Nif20
                            };

                var proveedores = query.ToList();

                List<umedidaModel> medidas = new List<umedidaModel>();

                foreach (var prov in proveedores)
                {
                    var queryart = from art in _contextdb2.Articulos1
                                   join artcl in _contextdb2.Articuloscamposlibres on art.Codarticulo equals artcl.Codarticulo
                                   into gj
                                   from subartcl in gj.DefaultIfEmpty()
                                   join prec in _contextdb2.Precioscompras on art.Codarticulo equals prec.Codarticulo
                                   into gj2
                                   from subprec in gj2.DefaultIfEmpty()
                                   where subartcl != null && subartcl.Planeacion == "T" && subprec.Codproveedor == prov.codproveedor
                                   select new
                                   {
                                       cod = art.Codarticulo,
                                       descripcion = art.Descripcion,
                                       precio = subprec.Pbruto,
                                       referencia = art.Referenciasprovs,
                                       tipoimpuesto = art.Impuestocompra
                                   };
                    var articulos = queryart.ToList();

                    foreach (var articulo in articulos)
                    {
                        var umedidaart = _contextdb2.ItProductos.Where(x => x.Rfc == prov.rfc && x.Codarticulo == articulo.cod).FirstOrDefault();

                        if (umedidaart != null)
                        {
                            medidas.Add(new umedidaModel()
                            {
                                rfc = umedidaart.Rfc,
                                noIdentificacion = umedidaart.NoIdentificacion,
                                codarticulo = umedidaart.Codarticulo,
                                umedida = umedidaart.Umedida,
                                uds = umedidaart.Uds,
                                nomarticulo = articulo.descripcion,
                                nomprov = prov.nombre
                            });
                        }

                    }

                }
                return StatusCode(StatusCodes.Status200OK, medidas);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.ToString() });
            }


        }


        [HttpGet]
        [Route("GetMedidaUds/{proveedor}/{codarticulo}")]
        public async Task<ActionResult> getumedida(string proveedor, int codarticulo)
        {
            try
            {
                var umedida = _contextdb2.ItProductos.Where(x => x.Rfc == proveedor && x.Codarticulo == codarticulo).FirstOrDefault();
                return StatusCode(StatusCodes.Status200OK, umedida);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.ToString() });
            }


        }


        [HttpPost]
        [Route("UpdateMedidaUds")]
        public async Task<ActionResult> updateumedida([FromBody] itproductoModel model)
        {
            try
            {
                var umedida = _contextdb2.ItProductos.Where(x => x.Rfc == model.rfc && x.Codarticulo == model.codarticulo).FirstOrDefault();
                umedida.Umedida = model.umedida;
                umedida.Uds = model.uds;
                _contextdb2.ItProductos.Update(umedida);
                await _contextdb2.SaveChangesAsync();
                return StatusCode(StatusCodes.Status200OK, umedida);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.ToString() });
            }


        }

        [HttpDelete]
        [Route("DeleteMedidaUds/{id}/")]
        public async Task<ActionResult> deleteumedida(string id)
        {
            try
            {
                var umedida = _contextdb2.ItProductos.Where(x => x.NoIdentificacion == id).FirstOrDefault();
                _contextdb2.Remove(umedida);
                await _contextdb2.SaveChangesAsync();
                return StatusCode(StatusCodes.Status200OK, umedida);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.ToString() });
            }


        }

        [HttpGet]
        [Route("getPedidos/{idu}")]
        public async Task<ActionResult> GetPedidosBD(int idu)
        {
            try
            {
                var asignaciones = _dbpContext.AsignacionProvs.Where(x =>x.Idu == idu).ToList();
                List<Pedidos> pedidos = new List<Pedidos>();
                var pedidosdb = _dbpContext.Pedidos.Where(x => x.Fecha.Value.Date == DateTime.Now.Date && (x.Estatus.Equals("POR ACEPTAR") || x.Estatus.Equals("INCOMPLETO")) && x.Temporal != true).ToList();

                foreach (var item in pedidosdb)
                {
                    if (asignaciones.Any(x => x.Idprov == item.Proveedor && x.Idsuc == int.Parse(item.Sucursal)))
                    {
                        Pedidos p = JsonConvert.DeserializeObject<Pedidos>(item.Jdata);
                        p.id = item.Id;
                        pedidos.Add(p);
                    }

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
        public async Task<ActionResult> GetPedidosBDFecha([FromForm] DateTime fecha, [FromForm] int idu)
        {
            try
            {
               var asignaciones = _dbpContext.AsignacionProvs.Where(x => x.Idu == idu).ToList();
                List<Pedidos> pedidos = new List<Pedidos>();
                //var pedidosdb = _dbpContext.Pedidos.ToList();
                var pedidosdb = _dbpContext.Pedidos.Where(x => x.Fecha.Value.Date == fecha.Date && (x.Estatus.Equals("POR ACEPTAR") || x.Estatus.Equals("INCOMPLETO")) && x.Temporal != true).ToList();

                foreach (var item in pedidosdb)
                {
                    if (asignaciones.Any(x => x.Idprov == item.Proveedor && x.Idsuc == int.Parse(item.Sucursal)))
                    {
                        Pedidos p = JsonConvert.DeserializeObject<Pedidos>(item.Jdata);
                        p.id = item.Id;
                        pedidos.Add(p);
                    }
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
        [Route("getPedidosFechaH")]
        public async Task<ActionResult> GetPedidosBDFechaH([FromForm] DateTime fechai, [FromForm] DateTime fechaf)
        {
            try
            {
                bool notificacionhoy = false;
                Notificacione datanot = new Notificacione();
                DateTime hoy = DateTime.Now;
                var notifiacion = _dbpContext.Notificaciones.OrderByDescending(n => n.Fecha).FirstOrDefault();
                if (notifiacion == null)
                {
                    notificacionhoy = false;
                }
                else
                {
                    notificacionhoy = true;
                    datanot = notifiacion;
                }

                List<PedidosH> pedidos = new List<PedidosH>();
                //var pedidosdb = _dbpContext.Pedidos.ToList();
                var pedidosdb = _dbpContext.Pedidos.Where(x => x.Fecha.Value.Date >= fechai.Date && x.Fecha.Value.Date <= fechaf.Date && (x.Estatus.Equals("AUTORIZADO") || x.Estatus.Equals("RECHAZADO"))).ToList();

                foreach (var item in pedidosdb)
                {

                    PedidosH p = JsonConvert.DeserializeObject<PedidosH>(item.Jdata);
                    p.id = item.Id;
                    p.fechapedido = item.Fecha.Value;
                    p.numpedido = item.Numpedido;

                    if (item.Datam == null)
                    {
                        p.notificado = false;
                    }
                    else
                    {
                        DateTime fechaautorizacion = DateTime.ParseExact(item.Datam, "o", null, DateTimeStyles.RoundtripKind);
                        if (notificacionhoy && fechaautorizacion < datanot.Fecha)
                        {
                            p.notificado = true;
                        }
                        else
                        {
                            p.notificado = false;
                        }
                    }

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
        [Route("getHistorialPedidos")]
        public async Task<ActionResult> GetHPedidosBD()
        {
            try
            {
                bool notificacionhoy = false; 
                Notificacione datanot = new Notificacione();
                DateTime hoy = DateTime.Now;
                var notifiacion = _dbpContext.Notificaciones.Where(x => x.Fecha.Date == hoy.Date && x.Success == true).ToList();
                if (notifiacion.Count == 0)
                {
                    notificacionhoy = false; 
                }
                else
                {
                    notificacionhoy = true; 
                    datanot = notifiacion.OrderByDescending(e => e.Fecha).First();
                }

                List<PedidosH> pedidos = new List<PedidosH>();
                //var pedidosdb = _dbpContext.Pedidos.ToList();
                var pedidosdb = _dbpContext.Pedidos.Where(x => x.Fecha.Value.Date == DateTime.Now.Date && (x.Estatus.Equals("AUTORIZADO") || x.Estatus.Equals("RECHAZADO"))).ToList();

                foreach (var item in pedidosdb)
                {
                
                    PedidosH p = JsonConvert.DeserializeObject<PedidosH>(item.Jdata);
                    p.id = item.Id;
                    p.fechapedido = item.Fecha.Value;
                    p.numpedido = item.Numpedido;

                    if (item.Datam == null)
                    {
                        p.notificado = false;
                    }
                    else
                    {
                        DateTime fechaautorizacion = DateTime.ParseExact(item.Datam, "o", null, DateTimeStyles.RoundtripKind);
                        if (notificacionhoy && fechaautorizacion < datanot.Fecha)
                        {
                            p.notificado = true;
                        }
                        else
                        {
                            p.notificado = false; 
                        }
                    }

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
        [Route("UpdatePedido")]
        public async Task<ActionResult> updatepedido([FromBody] ModificacionesModel model)
        {
            try
            {

                var pedidodb = _dbpContext.Pedidos.Find(model.id);
                if (pedidodb != null)
                {
                    string valorantes = "";
                    Pedidos p = JsonConvert.DeserializeObject<Pedidos>(pedidodb.Jdata);
                    foreach (var art in p.articulos)
                    {
                        if (art.codArticulo == model.codarticulo)
                        {
                            valorantes = art.inventariohoy.ToString();
                        }
                    }


                    int status = 1;
                    var pedido = JsonConvert.DeserializeObject<Pedidos>(pedidodb.Jdata);

                    var articulopedido = pedido.articulos.Where(x => x.codArticulo == model.codarticulo).FirstOrDefault();
                    var consumospromedio = articulopedido.consumospromedios;
                    int[] arraycal = articulopedido.arraycalendario;
                    double consumopedido = 0;
                    for (int i = 0; i < arraycal.Length; i++)
                    {
                        if (arraycal[i] > 0)
                        {
                            if (articulopedido.diasespeciales[i] == null)
                            {
                                consumopedido += consumospromedio[i].consumo;
                            }
                            else
                            {
                                double factor = articulopedido.diasespeciales[i].FactorConsumo ?? 1.5; 
                                consumopedido += (consumospromedio[i].consumo * factor);
                            }

                        }
                    }
                    double inventario = 0;
                    if (model.inventario > 0)
                    {
                        inventario = model.inventario;
                        valorantes = articulopedido.inventariohoy.ToString();
                        articulopedido.hayinventario = true; 
                    }
                    else { inventario = articulopedido.inventariohoy; }

                    var parametros = _dbpContext.Parametros.FirstOrDefault();
                    dynamic obj = JsonConvert.DeserializeObject<dynamic>(parametros.Jdata);
                    double factorstock = (double)obj.pedido.factorstock;

                    double proyeccion = (consumopedido + (articulopedido.consumomayor * factorstock) - inventario);

                    if (articulopedido.calendarioespecial) 
                    {
                        proyeccion = proyeccion - articulopedido.unidadesextra; 
                    }

                    int iva = 0;
                    var itprod = _contextdb2.ItProductos.Where(p => p.Rfc == pedido.rfc && p.Codarticulo == articulopedido.codArticulo).FirstOrDefault();
                    Boolean tienemultiplo = itprod == null ? false : true;
                    if (!tienemultiplo) { status = 2; }
                    double unidadescaja = itprod == null ? 1 : (double)itprod.Uds;
                    int cajas = 0;
                    if (proyeccion % unidadescaja == 0)
                    {
                        cajas = (int)(proyeccion / unidadescaja);
                    }
                    else
                    {
                        double resultado = proyeccion / unidadescaja;
                        cajas = (int)Math.Floor(resultado) + 1;
                    }
                    double unidades_totales = cajas * unidadescaja;
                    double total_linea = (double)(unidades_totales * articulopedido.precio);

                    articulopedido.inventariohoy = inventario;
                    articulopedido.cajas = cajas;
                    articulopedido.unidadescaja = unidadescaja;
                    articulopedido.unidadestotales = unidades_totales;
                    articulopedido.total_linea = total_linea;

                    double totalpedido = 0;
                    foreach (var art in pedido.articulos)
                    {

                        totalpedido += art.total_linea;
                        if (art.hayinventario == false) { status = 2;  }
                    }

                    pedido.total = totalpedido; 
                    pedido.status = status;
                    pedidodb.Jdata = JsonConvert.SerializeObject(pedido);
                    pedidodb.Estatus = status == 1 ? "POR ACEPTAR" : "INCOMPLETO";

                    _dbpContext.Pedidos.Update(pedidodb);
                    await _dbpContext.SaveChangesAsync();

                    if (model.inventario > 0)
                    {
                        _dbpContext.Modificaciones.Add(
                         new Modificacione()
                         {
                             Modificacion = "INVENTARIO",
                             ValAntes = valorantes,
                             ValDespues = model.inventario.ToString(),
                             Justificacion = model.justificacion,
                             Fecha = DateTime.Now,
                             Idusuario = model.idusuario,
                             IdPedido = pedidodb.Id,
                             Codarticulo = model.codarticulo,
                             Enviado = false
                         }
                           );
                        await _dbpContext.SaveChangesAsync();
                    }



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
                
                var pedidodb = _dbpContext.Pedidos.Find(idp);
                SqlConnection connection = (SqlConnection)_dbpContext.Database.GetDbConnection();
                connection.Open();

                SqlTransaction transaccion = connection.BeginTransaction();

                var pedido = JsonConvert.DeserializeObject<Pedidos>(pedidodb.Jdata);
                var cajafront = _contextdb2.RemCajasfronts.Where(x => x.Cajafront == 1 && x.Idfront == int.Parse(pedido.idSucursal)).FirstOrDefault();
                var transporte = _contextdb2.Transportes.Where(x => x.Fax == pedidodb.Sucursal).FirstOrDefault();
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
                        _dbpContext.Pedidos.Update(pedidodb);
                            await _dbpContext.SaveChangesAsync();

                            // revisar auditorias  
                            var modificaciones = _dbpContext.Modificaciones.Where(x => x.IdPedido == idp).ToList();

                            foreach (var item in modificaciones)
                            {
                                item.PedidoSerie = numserie.ToString();
                                item.Numpedido = numpedido;
                                item.Enviado = true;
                                _dbpContext.Modificaciones.Update(item);
                                await _dbpContext.SaveChangesAsync();
                            }

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


        [HttpGet]
        [Route("rechazarPedido/{idp}")]
        public async Task<ActionResult> rechazar(int idp)
        {
            try
            {
                var pedidodb = _dbpContext.Pedidos.Where(p => p.Id == idp).FirstOrDefault();
                if (pedidodb != null)
                {
                    var pedido = JsonConvert.DeserializeObject<Pedidos>(pedidodb.Jdata);
                    pedido.status = 4;
                    pedidodb.Jdata = JsonConvert.SerializeObject(pedido);
                    pedidodb.Estatus = "RECHAZADO";
                    _dbpContext.Pedidos.Update(pedidodb);
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
        [Route("rechazarPedidos")]
        public async Task<ActionResult> rechazarpedidos([FromForm] string jdata)
        {
            try
            { 
                int[] arrpedidos = System.Text.Json.JsonSerializer.Deserialize<int[]>(jdata);

                foreach (var idp in arrpedidos) 
                {

                    var pedidodb = _dbpContext.Pedidos.Where(p => p.Id == idp).FirstOrDefault();
                    if (pedidodb != null)
                    {
                        var pedido = JsonConvert.DeserializeObject<Pedidos>(pedidodb.Jdata);
                        pedido.status = 4;
                        pedidodb.Jdata = JsonConvert.SerializeObject(pedido);
                        pedidodb.Estatus = "RECHAZADO";
                        _dbpContext.Pedidos.Update(pedidodb);
                        await _dbpContext.SaveChangesAsync();
                    }
                }

                return StatusCode(StatusCodes.Status200OK);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.ToString() });
            }


        }


        [HttpPost]
        [Route("guardarParametros")]
        public async Task<ActionResult> guardarParametros([FromBody] ModelsDBP.Parametro model)
        {
            try
            {
                var parametros = _dbpContext.Parametros.ToList();
                if (parametros.Count == 0)
                {
                    _dbpContext.Parametros.Add(new ModelsDBP.Parametro()
                    {
                        Jdata = model.Jdata
                    });
                    await _dbpContext.SaveChangesAsync();
                }
                else 
                {
                    var parametro = parametros[0];
                    parametro.Jdata = model.Jdata;
                    _dbpContext.Parametros.Update(parametro);
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
        [Route("getParametros")]
        public async Task<ActionResult> getParametros()
        {
            try
            {
                var parametro = _dbpContext.Parametros.FirstOrDefault(); 
               
                return StatusCode(StatusCodes.Status200OK,parametro);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.ToString() });
            }


        }


        [HttpPost]
        [Route("AjusteCompras")]
        public async Task<ActionResult> ajustecompras([FromBody] AjusteComprasModel model)
        {
            try
            {
                var pedidodb = _dbpContext.Pedidos.Where(p => p.Id == model.id).FirstOrDefault();
                    var pedido = JsonConvert.DeserializeObject<Pedidos>(pedidodb.Jdata);
                var articulo = pedido.articulos.Where(x => x.codArticulo == model.codarticulo).FirstOrDefault();
                    var registro = _dbpContext.Modificaciones.Where(x => x.IdPedido == model.id && x.Modificacion == "AJUSTE COMPRAS" && x.Codarticulo == model.codarticulo).FirstOrDefault();

                if (registro == null)
                {
                    _dbpContext.Modificaciones.Add(new Modificacione()
                    {
                        Modificacion = "AJUSTE COMPRAS",
                        ValAntes = articulo.unidadestotales.ToString(),
                        ValDespues = model.unidades.ToString(),
                        Justificacion = model.justificacion,
                        Fecha = DateTime.Now,
                        Idusuario = model.idusuario,
                        IdPedido = model.id,
                        Enviado = false,
                        PedidoSerie = "",
                        Numpedido = 0,
                        Codarticulo = model.codarticulo,
                        Comentario = model.comentario
                        
                    });

                    await _dbpContext.SaveChangesAsync();  

                }
                else 
                {
                    registro.ValDespues = model.cajas.ToString(); 
                    registro.Justificacion = model.justificacion;   
                    registro.Fecha = DateTime.Now;
                    _dbpContext.Modificaciones.Update(registro);
                    await _dbpContext.SaveChangesAsync();
                }

                foreach (var art in pedido.articulos)
                {
                    if (art.codArticulo == model.codarticulo)
                    {
                        art.cajas = model.cajas;
                        art.unidadestotales = model.unidades;
                        art.total_linea = (double)(art.precio * model.unidades);

                    }
                }

                double totalpedido = 0;
                foreach (var art in pedido.articulos)
                {

                    totalpedido += art.total_linea;
                }
                pedido.total = totalpedido;
                pedidodb.Jdata = JsonConvert.SerializeObject(pedido);
                _dbpContext.Pedidos.Update(pedidodb);
                await _dbpContext.SaveChangesAsync();

                return StatusCode(StatusCodes.Status200OK);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.ToString() });
            }


        }



        [HttpGet]
        [Route("estatusAjusteC/{idp}/{codarticulo}")]
        public async Task<ActionResult> estausajustex(int idp, int codarticulo)
        {
            try
            {  
                var registro = _dbpContext.Modificaciones.Where(x => x.IdPedido == idp && x.Modificacion == "AJUSTE COMPRAS" && x.Codarticulo == codarticulo).FirstOrDefault();
                return StatusCode(StatusCodes.Status200OK,registro);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.ToString() });
            }


        }

        [HttpGet]
        [Route("BorrarAjusteC/{idp}/{codarticulo}")]
        public async Task<ActionResult> borrarajustex(int idp, int codarticulo)
        {
            try
            {
                var pedidodb = _dbpContext.Pedidos.Where(p => p.Id == idp).FirstOrDefault();
                var pedido = JsonConvert.DeserializeObject<Pedidos>(pedidodb.Jdata);
                var articulo = pedido.articulos.Where(x => x.codArticulo == codarticulo).FirstOrDefault();
                var registro = _dbpContext.Modificaciones.Where(x => x.IdPedido == idp && x.Modificacion == "AJUSTE COMPRAS" && x.Codarticulo == codarticulo).FirstOrDefault();

                foreach (var art in pedido.articulos)
                {
                    if (art.codArticulo == codarticulo)
                    {
                        int cajas =(int)(double.Parse(registro.ValAntes) / art.unidadescaja); 
                        art.cajas = cajas;
                        art.unidadestotales = double.Parse(registro.ValAntes);
                        art.total_linea = (double)(art.precio * double.Parse(registro.ValAntes));

                        //var regalmacenaje = _dbpContext.Almacenajes.Where(x => x.Idsucursal == int.Parse(pedido.idSucursal) && x.Codarticulo == art.codArticulo).FirstOrDefault();
                        //Boolean tienelimitealmacen = false;
                        //double porcentajeAlmacen = 0;

                        //if (regalmacenaje != null)
                        //{
                        //    tienelimitealmacen = true;
                        //    porcentajeAlmacen = (art.unidadestotales / regalmacenaje.Capacidad) * 100.00;
                        //}

                        //art.tienelimitealmacen = tienelimitealmacen;
                        //art.porcentajeCapacidad = porcentajeAlmacen;
                    }
                }

                double totalpedido = 0;
                foreach (var art in pedido.articulos)
                {

                    totalpedido += art.total_linea;
                }
                pedido.total = totalpedido;
                pedidodb.Jdata = JsonConvert.SerializeObject(pedido);
                _dbpContext.Pedidos.Update(pedidodb);
                await _dbpContext.SaveChangesAsync();
                if (registro != null)
                {
                    _dbpContext.Modificaciones.Remove(registro);
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
        [Route("ExcelHistorialpedidos")]
        public IActionResult GenerateExcel([FromForm] string jdata)
        {
          
            List<PedidosH> pedidos = JsonConvert.DeserializeObject<List<PedidosH>>(jdata);
            Color colorcelda = ColorTranslator.FromHtml("#00000000");
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            // Crear un nuevo archivo de Excel
            using (var package = new ExcelPackage())
            {
                // Agregar una hoja al libro de trabajo
                var worksheet = package.Workbook.Worksheets.Add("Hoja 1");

                worksheet.Cells[1, 1].Value = "NUM PEDIDO";
                worksheet.Cells[1, 2].Value = "SUCURSAL";
                worksheet.Cells[1, 3].Value = "PROVEEDOR";
                worksheet.Cells[1, 4].Value = "ARTICULO";
                worksheet.Cells[1, 5].Value = "CAJAS";
                worksheet.Cells[1, 6].Value = "UDS/CAJA";
                worksheet.Cells[1, 7].Value = "UDS";
                worksheet.Cells[1, 8].Value = "IMPUESTO";
                worksheet.Cells[1, 9].Value = "PRECIO";
                worksheet.Cells[1, 10].Value = "TOTAL";
                worksheet.Cells[1, 11].Value = "FECHA";
                worksheet.Cells[1, 12].Value = "ENTREGA";
                worksheet.Cells[1, 13].Value = "ESTATUS";


                using (var range = worksheet.Cells["A1:M1"])
                {   
                    Color colorFondo = ColorTranslator.FromHtml("#00000000");
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(colorFondo);
                    range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    range.AutoFitColumns();
                }
                int contador = 2;
                for (int i = 0; i < pedidos.Count; i++)
                {
                    foreach (var art in pedidos[i].articulos) 
                    {
                        string estatus = "";
                        if (pedidos[i].status == 3) 
                        {
                            estatus = "AUTORIZADO";
                        }
                        if (pedidos[i].status == 4)
                        {
                            estatus = "RECHAZADO";
                        }
                        worksheet.Cells[contador, 1].Value = pedidos[i].numpedido;
                        worksheet.Cells[contador, 2].Value = pedidos[i].nombresucursal;
                        worksheet.Cells[contador, 3].Value = pedidos[i].nombreproveedor;
                        worksheet.Cells[contador, 4].Value = art.nombre;
                        worksheet.Cells[contador, 5].Value = art.cajas;
                        worksheet.Cells[contador, 6].Value = art.unidadescaja;
                        worksheet.Cells[contador, 7].Value = art.unidadestotales;
                        worksheet.Cells[contador, 8].Value = art.tipoImpuesto;
                        worksheet.Cells[contador, 9].Value = art.precio;
                        worksheet.Cells[contador, 10].Value = art.total_linea;
                        worksheet.Cells[contador, 11].Value = pedidos[i].fechapedido;
                        worksheet.Cells[contador, 11].Style.Numberformat.Format = "yyyy-mm-dd";

                        worksheet.Cells[contador, 12].Value = pedidos[i].fechaEntrega;
                        worksheet.Cells[contador, 12].Style.Numberformat.Format = "yyyy-mm-dd";

                        worksheet.Cells[contador, 13].Value = estatus;
                        contador++; 
                    }
                }
                

                using (var range = worksheet.Cells)
                {
                    range.AutoFitColumns();
                }

                // Configurar la respuesta HTTP para devolver el archivo de Excel
                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                var byteArray = stream.ToArray();
                var base64String = Convert.ToBase64String(byteArray);

                return Ok(new { base64File = base64String });
            }
        }


        [HttpPost]
        [Route("ExcelPedidos")]
        public IActionResult GenerateExcelPedidos([FromForm] string jdata)
        {

            List<Pedidos> pedidos = JsonConvert.DeserializeObject<List<Pedidos>>(jdata);
            Color colorcelda = ColorTranslator.FromHtml("#00000000");
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            // Crear un nuevo archivo de Excel
            using (var package = new ExcelPackage())
            {
                // Agregar una hoja al libro de trabajo
                var worksheet = package.Workbook.Worksheets.Add("Hoja 1");

                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "SUCURSAL";
                worksheet.Cells[1, 3].Value = "PROVEEDOR";
                worksheet.Cells[1, 4].Value = "ARTICULO";
                worksheet.Cells[1, 5].Value = "CAJAS";
                worksheet.Cells[1, 6].Value = "UDS/CAJA";
                worksheet.Cells[1, 7].Value = "UDS";
                worksheet.Cells[1, 8].Value = "IMPUESTO";
                worksheet.Cells[1, 9].Value = "PRECIO";
                worksheet.Cells[1, 10].Value = "TOTAL";
                worksheet.Cells[1, 11].Value = "ENTREGA";
                worksheet.Cells[1, 12].Value = "ESTATUS";


                using (var range = worksheet.Cells["A1:L1"])
                {
                    Color colorFondo = ColorTranslator.FromHtml("#00000000");
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(colorFondo);
                    range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    range.AutoFitColumns();
                }
                int contador = 2;
                for (int i = 0; i < pedidos.Count; i++)
                {
                    foreach (var art in pedidos[i].articulos)
                    {
                        string estatus = "";
                        if (pedidos[i].status == 1)
                        {
                            estatus = "POR ACEPTAR";
                        }
                        if (pedidos[i].status == 2)
                        {
                            estatus = "INCOMPLETO";
                        }
                        if (pedidos[i].status == 3)
                        {
                            estatus = "AUTORIZADO";
                        }
                        if (pedidos[i].status == 4)
                        {
                            estatus = "RECHAZADO";
                        }

                        worksheet.Cells[contador, 1].Value = pedidos[i].id; 
                        worksheet.Cells[contador, 2].Value = pedidos[i].nombresucursal;
                        worksheet.Cells[contador, 3].Value = pedidos[i].nombreproveedor;
                        worksheet.Cells[contador, 4].Value = art.nombre;
                        worksheet.Cells[contador, 5].Value = art.cajas;
                        worksheet.Cells[contador, 6].Value = art.unidadescaja;
                        worksheet.Cells[contador, 7].Value = art.unidadestotales;
                        worksheet.Cells[contador, 8].Value = art.tipoImpuesto;
                        worksheet.Cells[contador, 9].Value = art.precio;
                        worksheet.Cells[contador, 10].Value = art.total_linea;
                        worksheet.Cells[contador, 11].Value = pedidos[i].fechaEntrega;
                        worksheet.Cells[contador, 11].Style.Numberformat.Format = "yyyy-mm-dd";
                        worksheet.Cells[contador, 12].Value = estatus;
                        contador++;
                    }
                }


                using (var range = worksheet.Cells)
                {
                    range.AutoFitColumns();
                }

                // Configurar la respuesta HTTP para devolver el archivo de Excel
                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                var byteArray = stream.ToArray();
                var base64String = Convert.ToBase64String(byteArray);

                return Ok(new { base64File = base64String });
            }
        }


        [HttpPost]
        [Route("AceptarTodo")]
        public async Task<IActionResult> AceptarTodoAsync([FromForm] AceptarTodoModel model)
        {

            try
            {
                var asignaciones = _dbpContext.AsignacionProvs.Where(x => x.Idu == model.idu).ToList();

                List<Pedido> pedidosdb = new List<Pedido>();
                List<Pedido> pedidosdbU = new List<Pedido>();
                //var pedidosdb = _dbpContext.Pedidos.ToList();

                if (model.fecha == null)
                {
                    pedidosdb = _dbpContext.Pedidos.Where(x => x.Fecha.Value.Date == DateTime.Now.Date && (x.Estatus.Equals("POR ACEPTAR")) && x.Temporal != true).ToList();
                }
                else 
                {
                    pedidosdb = _dbpContext.Pedidos.Where(x => x.Fecha.Value.Date == model.fecha.Value.Date && (x.Estatus.Equals("POR ACEPTAR")) && x.Temporal != true).ToList();
                }

                if (model.proveedor > -1) 
                {
                    pedidosdb = pedidosdb.Where(x => x.Proveedor == model.proveedor).ToList(); 
                }

                if (model.sucursal > -1)
                {
                    pedidosdb = pedidosdb.Where(x => x.Sucursal == model.sucursal.ToString()).ToList();
                }

                foreach (var item in pedidosdb) 
                {
                    if (asignaciones.Any(x => x.Idprov == item.Proveedor && x.Idsuc == int.Parse(item.Sucursal)))
                    {
                        pedidosdbU.Add(item);
                    }
                }
                pedidosdb.Clear();
                pedidosdb = pedidosdbU; 

                    SqlConnection connection = (SqlConnection)_dbpContext.Database.GetDbConnection();
                connection.Open();
                foreach (var pedidodb in pedidosdb)
                {
                    var pedido = JsonConvert.DeserializeObject<Pedidos>(pedidodb.Jdata);

                   // if (pedido.total <= 0 || LineasEnNegativas(pedido.articulos) == true)
                        if (pedido.total<=0 || Funciones.LineasRojas(pedido.articulos, pedido.tieneretornables, pedido.cartones) == true)
                    {
                    }
                    else
                    {
                        SqlTransaction transaction = connection.BeginTransaction();
                            try
                            {
                                var cajafront = _contextdb2.RemCajasfronts.Where(x => x.Cajafront == 1 && x.Idfront == int.Parse(pedido.idSucursal)).FirstOrDefault();
                                var transporte = _contextdb2.Transportes.Where(x => x.Fax == pedidodb.Sucursal).FirstOrDefault();
                                int idtransporte = 0;
                                if (transporte != null) { idtransporte = transporte.Codigo; }
                                string numserie = cajafront.Cajamanager + "X";

                                string codalmacen = "";

                                if (int.Parse(pedido.idSucursal) < 10)
                                {
                                    codalmacen = "0" + pedido.idSucursal;
                                }
                                else { codalmacen = pedido.idSucursal; }

                               // string querynumped = "SELECT ISNULL(MAX(NUMPEDIDO), 0) AS numero_mayor FROM [BD2_PRUEBA].dbo.PEDCOMPRACAB WHERE NUMSERIE ='" + numserie + "'";
                                string querynumped = "SELECT ISNULL(MAX(NUMPEDIDO), 0) AS numero_mayor FROM [BD2].dbo.PEDCOMPRACAB WHERE NUMSERIE ='" + numserie + "'";
                                SqlCommand command = new SqlCommand(querynumped, connection, transaction);

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
                                command = new SqlCommand("SP_INSERT_PEDIDO", connection, transaction);
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
                                    command = new SqlCommand("SP_INSERT_PEDIDOLIN", connection, transaction);
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

                                    command = new SqlCommand("SP_INSERT_COMPRATOT", connection, transaction);
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
                                command = new SqlCommand("SP_INSERT_TESORERIA", connection, transaction);
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

                                command = new SqlCommand("SP_UPDATE_SERIESDOC", connection, transaction);
                                command.CommandType = CommandType.StoredProcedure;

                                // Agregar parámetros al procedimiento almacenado
                                command.Parameters.AddWithValue("@SERIE", numserie);
                                //command.Parameters.AddWithValue("@SERIE", "asrtdenjic87cui");
                            // Ejecutar el procedimiento almacenado
                            command.ExecuteNonQuery();

                            await transaction.CommitAsync(); 
                                pedido.status = 3;
                                pedidodb.Jdata = JsonConvert.SerializeObject(pedido);
                                pedidodb.Estatus = "AUTORIZADO";
                                pedidodb.Numpedido = supedido;
                                pedidodb.Datam = DateTime.Now.ToString("o"); 
                                _dbpContext.Pedidos.Update(pedidodb);
                                await _dbpContext.SaveChangesAsync();

                                // revisar auditorias  
                                var modificaciones = _dbpContext.Modificaciones.Where(x => x.IdPedido == pedidodb.Id).ToList();

                                foreach (var item in modificaciones)
                                {
                                    item.PedidoSerie = numserie.ToString();
                                    item.Numpedido = numpedido;
                                    item.Enviado = true;
                                    _dbpContext.Modificaciones.Update(item);
                                    await _dbpContext.SaveChangesAsync();
                                }

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
                            catch (Exception err)
                            {
                                await transaction.RollbackAsync(); 
                            }
                    }

                }
                connection.Close();
                return StatusCode(StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.ToString() });
            }
        }


        [HttpGet]
        [Route("notificar/{resp}")]
        public async Task<ActionResult> notificar(int resp)
        {
            try
            {
                DateTime hoy = DateTime.Now;    
                Boolean success = false; 
                if (resp == 1) 
                {
                    success = true; 
                }

                _dbpContext.Notificaciones.Add(new Notificacione() 
                {
                    Fecha = hoy,
                    Success = success
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
        [Route("getNotificacion")]
        public async Task<ActionResult> getnotificacion()
        {
            try
            {
                DateTime hoy = DateTime.Now;
                var notifiacion = _dbpContext.Notificaciones.Where(x=> x.Fecha.Date == hoy.Date && x.Success == true).ToList();
                if (notifiacion.Count == 0)
                {
                    return NotFound();
                }
                else
                {
                    var data = notifiacion.OrderByDescending(e => e.Fecha).First();

                    return StatusCode(StatusCodes.Status200OK,data);
                }

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.ToString() });
            }


        }



        [HttpPost]
        [Route("EliminarLinea")]
        public async Task<ActionResult> EliminarLinea([FromForm] int idp, [FromForm] int codart)
        {
            try
            {

                var pedidodb = _dbpContext.Pedidos.Find(idp);
                if (pedidodb != null)
                {
                    string valorantes = "";
                    Pedidos p = JsonConvert.DeserializeObject<Pedidos>(pedidodb.Jdata);
                    p.articulos.RemoveAll(a => a.codArticulo == codart);
                    pedidodb.Jdata = JsonConvert.SerializeObject(p);


                    // recalcular el total del pedido 
                    int status = 1;
                    double totalpedido = 0;
                    foreach (var art in p.articulos)
                    {
                        totalpedido += art.total_linea;
                        if (art.hayinventario == false || art.tienemultiplo == false)
                        {
                            status = 2;
                        }
                    }
                    p.total = totalpedido;
                    p.status = status;
                    pedidodb.Jdata = JsonConvert.SerializeObject(p);
                    pedidodb.Estatus = status == 1 ? "POR ACEPTAR" : "INCOMPLETO";
                    _dbpContext.Pedidos.Update(pedidodb);
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
        [Route("EliminarLineas")]
        public async Task<ActionResult> EliminarLineas([FromForm] int idp, [FromForm] string articulosp, [FromForm] int idu)
        {
            try
            {
                int[] articulos = JsonConvert.DeserializeObject<int[]>(articulosp);
                var pedidodb = _dbpContext.Pedidos.Find(idp);
                if (pedidodb != null)
                {
                    string valorantes = "";
                    Pedidos p = JsonConvert.DeserializeObject<Pedidos>(pedidodb.Jdata);

                    foreach (int codart in articulos) 
                    {
                        var temparticulo = p.articulos.Where(x => x.codArticulo == codart).FirstOrDefault(); 
                        p.articulos.RemoveAll(a => a.codArticulo == codart);

                        // guardar en el log de modificaciones 
                        if (temparticulo.unidadestotales < 0)
                        {
                            _dbpContext.Modificaciones.Add(new Modificacione()
                            {
                                Modificacion = "AJUSTE LINEAS PEDIDO",
                                ValAntes = temparticulo.unidadestotales.ToString(),
                                ValDespues = "0",
                                Justificacion = "BORRAR LINEA DEL PEDIDO",
                                Fecha = DateTime.Now,
                                Idusuario = idu,
                                IdPedido = idp,
                                Enviado = false,
                                Codarticulo = codart,
                                Comentario = "STOCK POR ENCIMA DEL CONSUMO PROYECTADO"
                            }); ;
                            await _dbpContext.SaveChangesAsync();
                        }
                        else
                        {
                            _dbpContext.Modificaciones.Add(new Modificacione()
                            {
                                Modificacion = "AJUSTE LINEAS PEDIDO",
                                ValAntes = temparticulo.unidadestotales.ToString(),
                                ValDespues = "0",
                                Justificacion = "BORRAR LINEA DEL PEDIDO",
                                Fecha = DateTime.Now,
                                Idusuario = idu,
                                IdPedido = idp,
                                Enviado = false,
                                Codarticulo = codart,
                                Comentario = "STOCK EXACTO PARA CUBRIR VENTA"
                            }); ;
                            await _dbpContext.SaveChangesAsync();
                        }
                    }

                    // recalcular el total del pedido 
                    int status = 1; 
                    double totalpedido = 0;
                    foreach (var art in p.articulos)
                    {
                        totalpedido += art.total_linea;
                        if (art.hayinventario == false || art.tienemultiplo == false) 
                        {
                            status = 2; 
                        }
                    }
                    p.total = totalpedido;
                    p.status = status;
                    pedidodb.Jdata = JsonConvert.SerializeObject(p);
                    pedidodb.Estatus = status == 1 ? "POR ACEPTAR" : "INCOMPLETO"; 
                    _dbpContext.Pedidos.Update(pedidodb);
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
        [Route("StatusPedidos/{idu}")]
        public async Task<ActionResult> statusPedidos(int idu)
        {
            try
            {
                var estatus = _dbpContext.ValidacionPedidos.Where(x => x.Idu == idu).FirstOrDefault(); 
                var pedidoshoy = _dbpContext.Pedidos.Where(x => x.Fecha.Value.Date == DateTime.Now.Date).ToList();
                int est = 0; 
                if (estatus != null)
                {
                    est = 1; 
                }

                return StatusCode(StatusCodes.Status200OK, new { status = est });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.ToString() });
            }

        }


        [HttpPost]
        [Route("UpdateCartonesPedido")]
        public async Task<ActionResult> UpdateCartonesPedidos([FromForm]int idp, [FromForm] double cartones, [FromForm] string justificacion, [FromForm] int idu)
        {
            try
            {
                var pedidodb = _dbpContext.Pedidos.Find(idp);
                if (pedidodb != null)
                {
                    string valorantes = "";
                    Pedidos p = JsonConvert.DeserializeObject<Pedidos>(pedidodb.Jdata);

                    // valor antes de los cartones 
                    _dbpContext.Modificaciones.Add(
                        new Modificacione()
                        {
                            Modificacion = "CARTONES",
                            ValAntes = p.cartones.ToString(),
                            ValDespues = cartones.ToString(),
                            Justificacion = justificacion,
                            Fecha = DateTime.Now,
                            Idusuario = idu,
                            IdPedido = pedidodb.Id,
                            Codarticulo = 10277,
                            Enviado = false
                        }
                          );
                    await _dbpContext.SaveChangesAsync();

                    var pedido = JsonConvert.DeserializeObject<Pedidos>(pedidodb.Jdata);
                    pedido.cartones = cartones;
                    pedido.capturacartones = true; 
                    pedidodb.Jdata = JsonConvert.SerializeObject(pedido);

                    _dbpContext.Pedidos.Update(pedidodb);
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
        [Route("recalcularpedidos")]
        public async Task<ActionResult> recalcularpedidos([FromForm]int idu, [FromForm] int filtroproveedor, [FromForm] int filtrosucursal)
        {
            try
            {
                _dbpContext.ValidacionPedidos.Add(new ValidacionPedido() { Status = true, Idu = idu });
                await _dbpContext.SaveChangesAsync();
                var asignaciones = _dbpContext.AsignacionProvs.Where(x => x.Idu == idu).ToList();

                var parametros = _dbpContext.Parametros.FirstOrDefault();
                dynamic obj = JsonConvert.DeserializeObject<dynamic>(parametros.Jdata);


                List<Pedido> rangopedidosdel = new List<Pedido>();
                var delpedidos = _dbpContext.Pedidos.Where(x => x.Fecha.Value.Date == DateTime.Now.Date && (x.Estatus == "POR ACEPTAR" || x.Estatus == "INCOMPLETO") && x.Temporal != true).ToList();
                foreach (var item in delpedidos)
                {
                    if (asignaciones.Any(x => x.Idprov == item.Proveedor && x.Idsuc == int.Parse(item.Sucursal)))
                    {
                        rangopedidosdel.Add(item);
                    }
                }


                if (filtroproveedor > -999)
                {
                    rangopedidosdel = rangopedidosdel.Where(x => x.Proveedor == filtroproveedor).ToList();
                }

                if (filtrosucursal > -999)
                {
                    rangopedidosdel = rangopedidosdel.Where(x => x.Sucursal == filtrosucursal.ToString()).ToList();
                }

                foreach (var item in rangopedidosdel) 
                {
                    var modificaciones = _dbpContext.Modificaciones.Where(x => x.IdPedido == item.Id).ToList();
                    if (modificaciones.Count > 0) ;
                    _dbpContext.Modificaciones.RemoveRange(modificaciones);
                    await _dbpContext.SaveChangesAsync();
                }  

                _dbpContext.RemoveRange(rangopedidosdel);
                await _dbpContext.SaveChangesAsync();

                SqlConnection conn = (SqlConnection)_dbpContext.Database.GetDbConnection();
                conn.Open();
                List<Pedidos> pedidos = new List<Pedidos>();
                List<Calendario> calendarioshoy = new List<Calendario>();
                DateTime fechaHoy = DateTime.Now;
                DateTime fechaentrega = DateTime.Now;
                DayOfWeek diaSemana = fechaHoy.DayOfWeek;
                int numdia = 0;
                switch (diaSemana)
                {
                    case DayOfWeek.Monday:
                        numdia = 1;
                        break;
                    case DayOfWeek.Tuesday:
                        numdia = 2;
                        break;
                    case DayOfWeek.Wednesday:
                        numdia = 3;
                        break;
                    case DayOfWeek.Thursday:
                        numdia = 4;
                        break;
                    case DayOfWeek.Friday:
                        numdia = 5;
                        break;
                    case DayOfWeek.Saturday:
                        numdia = 6;
                        break;
                    case DayOfWeek.Sunday:
                        numdia = 0;
                        break;
                    default:
                        break;
                }
                var calendarios = _dbpContext.Calendarios.Where(x=> x.Temporal != true).ToList();
                foreach (var item in calendarios)
                {
                    int[][] array = JsonConvert.DeserializeObject<int[][]>(item.Jdata);
                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i][numdia] == 1)
                        {
                            if (asignaciones.Any(x => x.Idprov == item.Codproveedor && x.Idsuc == item.Codsucursal))
                            {
                                calendarioshoy.Add(item);
                            }
                        }
                    }
                }

                DateTime[] fechas = new DateTime[7];
                DateTime tempdt = fechaHoy.AddDays(-numdia);
                for (int i = 0; i < 7; i++)
                {
                    fechas[i] = tempdt;
                    tempdt = tempdt.AddDays(1);
                }
                // fechas[0] = fechas[0].AddDays(7); 

                if (filtroproveedor > -999)
                {
                    calendarioshoy = calendarioshoy.Where(x => x.Codproveedor == filtroproveedor).ToList(); 
                }

                if (filtrosucursal > -999)
                {
                    calendarioshoy = calendarioshoy.Where(x => x.Codsucursal == filtrosucursal).ToList();
                }

                foreach (var item in calendarioshoy)
                {
                    Boolean articulosdiferentes = false;
                    var haypedido = _dbpContext.Pedidos.Where(x => x.Fecha.Value.Date == DateTime.Now.Date && x.Proveedor == item.Codproveedor && x.Sucursal == item.Codsucursal.ToString() && x.Temporal != true).ToList();

                    Boolean generarpedido = true; 

                    foreach(var itemp in haypedido) 
                    {
                        if (itemp.Idcal == item.Id) 
                        {
                            generarpedido = false; 
                        }
                    }

                    if (haypedido.Count == 0 || generarpedido)
                    {
                        Boolean requierecartones = false;
                        double totalpedido = 0;
                        string nombresucursal = "";
                        string nombreproveedor = "";
                        int status = 1;
                        string rfcprov = "";
                        var prov = _contextdb2.Proveedores.Where(p => p.Codproveedor == item.Codproveedor).FirstOrDefault();
                        rfcprov = prov.Nif20;
                        var itemproveedor = _contextdb2.Proveedores.Where(p => p.Codproveedor == item.Codproveedor).FirstOrDefault();
                        nombreproveedor = itemproveedor.Nomproveedor;

                        var itemsucursal = _contextdb2.RemFronts.Where(s => s.Idfront == item.Codsucursal).FirstOrDefault();
                        nombresucursal = itemsucursal.Titulo;

                        Boolean inventarioteorico = false;
                        var reginvt = _dbpContext.InventarioTeoricos.Where(x => x.Idfront == item.Codsucursal).FirstOrDefault();
                        if (reginvt != null)
                        {
                            var regprovinvt = _dbpContext.InvTeoricoProveedores.Where(x => x.Codprov == item.Codproveedor).FirstOrDefault();
                            if (regprovinvt != null)
                            {
                                inventarioteorico = true;
                            }

                        }

                        int[][] array = JsonConvert.DeserializeObject<int[][]>(item.Jdata);
                        double consumopromedio = 0;

                        List<articuloModel> articulosl = new List<articuloModel>();
                        var articulosdb = _dbpContext.ArticulosProveedors.Where(x => x.Idcalendario == item.Id).ToList();
                        foreach (var artdb in articulosdb)
                        {
                            var tempq = from art in _contextdb2.Articulos1
                                        join artcl in _contextdb2.Articuloscamposlibres on art.Codarticulo equals artcl.Codarticulo
                                        into gj
                                        from subartcl in gj.DefaultIfEmpty()
                                        join prec in _contextdb2.Precioscompras on art.Codarticulo equals prec.Codarticulo
                                        into gj2
                                        from subprec in gj2.DefaultIfEmpty()
                                        where subartcl != null && subartcl.Codarticulo == artdb.Codarticulo && subprec.Codproveedor == item.Codproveedor
                                        select new
                                        {
                                            cod = art.Codarticulo,
                                            descripcion = art.Descripcion,
                                            precio = subprec.Pbruto,
                                            referencia = art.Referenciasprovs,
                                            tipoimpuesto = art.Impuestocompra
                                        };
                            var tempart = tempq.FirstOrDefault();
                            articulosl.Add(new articuloModel()
                            {
                                cod = tempart.cod,
                                descripcion = tempart.descripcion,
                                precio = (double)tempart.precio,
                                referencia = tempart.referencia,
                                tipoimpuesto = (int)tempart.tipoimpuesto
                            });

                        }


                        var query = from art in _contextdb2.Articulos1
                                    join artcl in _contextdb2.Articuloscamposlibres on art.Codarticulo equals artcl.Codarticulo
                                    into gj
                                    from subartcl in gj.DefaultIfEmpty()
                                    join prec in _contextdb2.Precioscompras on art.Codarticulo equals prec.Codarticulo
                                    into gj2
                                    from subprec in gj2.DefaultIfEmpty()
                                    where subartcl != null && subartcl.Planeacion == "T" && subprec.Codproveedor == item.Codproveedor
                                    select new articuloModel()
                                    {
                                        cod = art.Codarticulo,
                                        descripcion = art.Descripcion,
                                        precio = (double)subprec.Pbruto,
                                        referencia = art.Referenciasprovs,
                                        tipoimpuesto = (int)art.Impuestocompra
                                    };


                        int count = articulosl.Count;

                        var articulos = articulosl.Count > 0 ? articulosl : query.ToList();

                        List<ArticuloPedido> articulospedido = new List<ArticuloPedido>();
                        int numlinea = 0;
                        foreach (var art in articulos)
                        {
                            Boolean esretornable = false;
                            if (_dbpContext.Retornables.Where(x => x.Codart == art.cod).ToList().Count() > 0)
                            {
                                esretornable = true;
                                requierecartones = true;
                            }

                            fechaentrega = DateTime.Now;
                            numlinea++;
                            List<ConsumoModel> consumos = new List<ConsumoModel>();
                            consumos.Clear();

                            // Crear comando para ejecutar el procedimiento almacenado
                            using (SqlCommand command = new SqlCommand("SP_Consumo_Promedio", conn))
                            {
                                command.CommandType = CommandType.StoredProcedure;
                                string codalm = "";
                                if (item.Codsucursal < 10)
                                {
                                    codalm = "0" + item.Codsucursal;
                                }
                                else { codalm = item.Codsucursal.ToString(); }
                                // Agregar parámetros al procedimiento almacenado



                                int parametrosemanas = (int)obj.pedido.diasconprom;

                                command.Parameters.Add("@sucursal", SqlDbType.NVarChar).Value = codalm;
                                command.Parameters.Add("@articulo", SqlDbType.Int).Value = art.cod;
                                command.Parameters.Add("@semanas", SqlDbType.Int).Value = parametrosemanas;

                                try
                                {
                                    // Ejecutar el procedimiento almacenado
                                    SqlDataReader reader = command.ExecuteReader();

                                    while (reader.Read())
                                    {
                                        consumos.Add(new ConsumoModel
                                        {
                                            dia = int.Parse(reader["DIA"].ToString()),
                                            consumo = double.Parse(reader["CONSUMO"].ToString())
                                        });
                                    }

                                    reader.Close();
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Error al ejecutar el procedimiento almacenado: " + ex.Message);
                                }
                            }

                            if (consumos.Count < 7)
                            {
                                int iteraciones = 7 - consumos.Count;
                                for (int z = 0; z < iteraciones; z++)
                                {
                                    consumos.Add(new ConsumoModel()
                                    {
                                        dia = 0,
                                        consumo = 0
                                    });
                                }
                            }

                            consumopromedio = consumos[numdia].consumo;

                            double mayorconsumo = consumos.OrderByDescending(c => c.consumo).First().consumo;

                            double factorstock = (double)obj.pedido.factorstock;

                            double stockSeguridad = mayorconsumo * factorstock;

                            double consumopedido = 0;

                            int[] arraycal = new int[array[0].Length];
                            DiasEspecialesSucursal[] diasespeciales = { null, null, null, null, null, null, null };


                            for (int i = 0; i < array.Length; i++)
                            {
                                if (array[i][numdia] == 1)
                                {
                                    for (int j = 0; j < array[i].Length; j++)
                                    {

                                        if (array[i][j] == 1 || array[i][j] == 2 || array[i][j] == 3)
                                        {
                                            arraycal = array[i];
                                            var diaespecialsuc = _dbpContext.DiasEspecialesSucursals.ToList().Where(d => d.Fecha.Value.ToString("yyyy-MM-dd") == fechas[j].ToString("yyyy-MM-dd") && d.Sucursal == item.Codsucursal).FirstOrDefault();
                                            var diaespecial = _dbpContext.DiasEspeciales.ToList().Where(d => d.Fecha.ToString("yyyy-MM-dd") == fechas[j].ToString("yyyy-MM-dd")).FirstOrDefault();
                                            if (diaespecial == null && diaespecialsuc == null)
                                            {
                                                consumopedido += consumos[j].consumo;
                                            }
                                            else
                                            {
                                                if (diaespecialsuc == null)
                                                {
                                                    diasespeciales[j] = new DiasEspecialesSucursal()
                                                    {
                                                        Id = diaespecial.Id,
                                                        Dia = diaespecial.Dia,
                                                        Semana = diaespecial.Semana,
                                                        Fecha = diaespecial.Fecha,
                                                        Descripcion = diaespecial.Descripcion,
                                                        FactorConsumo = diaespecial.FactorConsumo,
                                                        Sucursal = 0
                                                    };
                                                    consumopedido += (consumos[j].consumo * diaespecial.FactorConsumo);
                                                }
                                                else
                                                {
                                                    int[] articulosdiesp = JsonConvert.DeserializeObject<int[]>(diaespecialsuc.Articulos);
                                                    if (articulosdiesp.Contains(art.cod))
                                                    {
                                                        diasespeciales[j] = diaespecialsuc;
                                                        double factor = (diaespecialsuc.FactorConsumo ?? 1.5);
                                                        consumopedido += (consumos[j].consumo * factor);
                                                    }
                                                    else
                                                    {
                                                        if (diaespecial != null)
                                                        {
                                                            diasespeciales[j] = new DiasEspecialesSucursal()
                                                            {
                                                                Id = diaespecial.Id,
                                                                Dia = diaespecial.Dia,
                                                                Semana = diaespecial.Semana,
                                                                Fecha = diaespecial.Fecha,
                                                                Descripcion = diaespecial.Descripcion,
                                                                FactorConsumo = diaespecial.FactorConsumo,
                                                                Sucursal = 0
                                                            };
                                                            consumopedido += (consumos[j].consumo * diaespecial.FactorConsumo);
                                                        }
                                                        else
                                                        {
                                                            consumopedido += consumos[j].consumo;
                                                        }
                                                    }

                                                }

                                            }

                                        }
                                    }
                                }

                            }

                            int countday = numdia;
                            for (int i = 0; i < 7; i++)
                            {
                                countday++;
                                fechaentrega = fechaentrega.AddDays(1);
                                if (countday == 7)
                                {
                                    countday = 0;
                                }

                                if (arraycal[countday] == 3)
                                {
                                    break;
                                }
                            }


                            List<PinventarioModel> inventarios = new List<PinventarioModel>();
                            inventarios.Clear();

                            if (inventarioteorico && DateTime.Now.Date != new DateTime(2024, 12, 26).Date && DateTime.Now.Date != new DateTime(2025, 1, 2).Date)
                            {
                                using (SqlCommand command = new SqlCommand("SPS_GET_DIFERENCIA_LIN", conn))
                                {
                                    command.CommandType = CommandType.StoredProcedure;
                                    string codalm = "";
                                    if (item.Codsucursal < 10)
                                    {
                                        codalm = "0" + item.Codsucursal;
                                    }
                                    else { codalm = item.Codsucursal.ToString(); }
                                    // Añadir parámetros al comando
                                    command.Parameters.Add("@FECHA", System.Data.SqlDbType.VarChar, 10).Value = DateTime.Now.ToString("dd/MM/yyyy");
                                    command.Parameters.Add("@CODALM", System.Data.SqlDbType.NVarChar, 10).Value = codalm;
                                    command.Parameters.Add("@CODART", System.Data.SqlDbType.Int).Value = art.cod;
                                    command.CommandTimeout = 120;

                                    // Ejecutar el comando y leer los resultados
                                    using (SqlDataReader reader = command.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            DateTime fecha = DateTime.Now;
                                            double unidades = (double)reader["INVFORMULA"];
                                            inventarios.Add(new PinventarioModel()
                                            {
                                                fecha = fecha,
                                                unidades = unidades,
                                            });
                                        }
                                    }
                                }
                            }
                            else
                            {

                                using (SqlCommand command = new SqlCommand("SP_GET_INVENTARIO", conn))
                                {
                                    command.CommandType = CommandType.StoredProcedure;
                                    string codalm = "";
                                    if (item.Codsucursal < 10)
                                    {
                                        codalm = "0" + item.Codsucursal;
                                    }
                                    else { codalm = item.Codsucursal.ToString(); }
                                    // Añadir parámetros al comando
                                    command.Parameters.Add("@sucursal", SqlDbType.NVarChar, 5).Value = codalm;
                                    command.Parameters.Add("@articulo", SqlDbType.Int).Value = art.cod;
                                    command.Parameters.Add("@FI", SqlDbType.NVarChar, 255).Value = DateTime.Now.ToString("yyyy-MM-dd");
                                    command.Parameters.Add("@FF", SqlDbType.NVarChar, 255).Value = DateTime.Now.ToString("yyyy-MM-dd");

                                    // Ejecutar el comando y leer los resultados
                                    using (SqlDataReader reader = command.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            DateTime fecha = (DateTime)reader["FECHA"];
                                            double unidades = reader.GetDouble(1);

                                            inventarios.Add(new PinventarioModel()
                                            {
                                                fecha = fecha,
                                                unidades = unidades,
                                            });
                                        }
                                    }
                                }

                            }

                            double inventario = 0;
                            Boolean hayinventario = false;
                            if (inventarios.Count > 0) { inventario = inventarios[0].unidades; hayinventario = true; } else { status = 2; }
                            double proyeccion = 0;
                            bool tieneudspendientes = false;
                            double unidadespendientes = 0;
                            if (item.Especial == true)
                            {
                                string fechastr = DateTime.Now.ToString("yyyy-MM-dd");
                                var pedidosentrega = _dbpContext.Pedidos.Where(x => x.Proveedor == item.Codproveedor && x.Sucursal == item.Codsucursal.ToString()
                                && x.Jdata.Contains("\"fechaEntrega\":\"" + fechastr) && x.Estatus == "AUTORIZADO").ToList();

                                foreach (var pe in pedidosentrega)
                                {
                                    Pedidos p = JsonConvert.DeserializeObject<Pedidos>(pe.Jdata);
                                    var temp = p.articulos.Where(x => x.codArticulo == art.cod).FirstOrDefault();
                                    if (temp != null)
                                    {
                                        tieneudspendientes = true;
                                        unidadespendientes = temp.unidadestotales;
                                        proyeccion = (consumopedido + stockSeguridad - inventario) - temp.unidadestotales;
                                    }
                                    else { proyeccion = (consumopedido + stockSeguridad - inventario); }
                                }

                                if (pedidosentrega.Count == 0)
                                {
                                    proyeccion = (consumopedido + stockSeguridad - inventario);
                                }

                            }
                            else
                            {
                                proyeccion = (consumopedido + stockSeguridad - inventario);
                            }
                            int iva = 0;
                            var itprod = _contextdb2.ItProductos.Where(p => p.Rfc == rfcprov && p.Codarticulo == art.cod).FirstOrDefault();
                            Boolean tienemultiplo = itprod == null ? false : true;
                            if (!tienemultiplo) { status = 2; }
                            double unidadescaja = itprod == null ? 1 : (double)itprod.Uds;
                            int cajas = 0;
                            if (proyeccion % unidadescaja == 0)
                            {
                                cajas = (int)(proyeccion / unidadescaja);
                            }
                            else
                            {
                                double resultado = proyeccion / unidadescaja;
                                cajas = (int)Math.Floor(resultado) + 1;
                            }
                            double unidades_totales = cajas * unidadescaja;
                            double total_linea = (double)(unidades_totales * art.precio);
                            totalpedido += total_linea;
                            var itemimpuesto = _contextdb2.Impuestos.Where(p => p.Tipoiva == art.tipoimpuesto).FirstOrDefault();
                            double ivaArt = (double)(itemimpuesto.Iva == null ? 16 : itemimpuesto.Iva);


                            var regalmacenaje = _dbpContext.Almacenajes.Where(x => x.Idsucursal == item.Codsucursal && x.Codarticulo == art.cod).FirstOrDefault();
                            Boolean tienelimitealmacen = false;
                            double unidadesentrega = 0;
                            double capacidadalm = 0; 
                            if (regalmacenaje != null)
                            {
                                capacidadalm = regalmacenaje.Capacidad;
                                string fechastr = DateTime.Now.ToString("yyyy-MM-dd");
                                var pedidosentrega = _dbpContext.Pedidos.Where(x => x.Proveedor == item.Codproveedor && x.Sucursal == item.Codsucursal.ToString()
                                && x.Jdata.Contains("\"fechaEntrega\":\"" + fechastr) && x.Estatus == "AUTORIZADO").ToList();

                                foreach (var pe in pedidosentrega)
                                {
                                    Pedidos p = JsonConvert.DeserializeObject<Pedidos>(pe.Jdata);
                                    var temp = p.articulos.Where(x => x.codArticulo == art.cod).FirstOrDefault();
                                    if (temp != null)
                                    {
                                        unidadesentrega = unidadesentrega + temp.unidadestotales;
                                    }
                                }

                                tienelimitealmacen = true;

                            }

                            articulospedido.Add(new ArticuloPedido()
                            {
                                codArticulo = art.cod,
                                nombre = art.descripcion,
                                inventariohoy = inventario,
                                precio = art.precio,
                                numlinea = numlinea,
                                cajas = cajas,
                                unidadescaja = unidadescaja,
                                unidadestotales = unidades_totales,
                                tipoImpuesto = (int)art.tipoimpuesto,
                                iva = ivaArt,
                                total_linea = total_linea,
                                codigoAlmacen = item.Codsucursal.ToString(),
                                tienemultiplo = tienemultiplo,
                                hayinventario = hayinventario,
                                consumospromedios = consumos,
                                consumomayor = mayorconsumo,
                                factorseguridad = 1.5,
                                arraycalendario = arraycal,
                                diasespeciales = diasespeciales,
                                calendarioespecial = tieneudspendientes,
                                unidadesextra = unidadespendientes,
                                esretornable = esretornable,
                                tienelimitealmacen = tienelimitealmacen,
                                capacidadalmfinal = (capacidadalm - unidadesentrega),
                                invformulado = inventarioteorico
                            });
                        }

                        // validar si requiere cartones 
                        double cartones = 0;
                        Boolean cartonescapturados = false;
                        if (requierecartones)
                        {
                            List<PinventarioModel> invcartones = new List<PinventarioModel>();
                            using (SqlCommand command = new SqlCommand("SP_GET_INVENTARIO", conn))
                            {
                                command.CommandType = CommandType.StoredProcedure;
                                string codalm = "";
                                if (item.Codsucursal < 10)
                                {
                                    codalm = "0" + item.Codsucursal;
                                }
                                else { codalm = item.Codsucursal.ToString(); }
                                // Añadir parámetros al comando
                                command.Parameters.Add("@sucursal", SqlDbType.NVarChar, 5).Value = codalm;
                                command.Parameters.Add("@articulo", SqlDbType.Int).Value = 10277;
                                command.Parameters.Add("@FI", SqlDbType.NVarChar, 255).Value = DateTime.Now.ToString("yyyy-MM-dd");
                                command.Parameters.Add("@FF", SqlDbType.NVarChar, 255).Value = DateTime.Now.ToString("yyyy-MM-dd");

                                // Ejecutar el comando y leer los resultados
                                using (SqlDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        DateTime fecha = (DateTime)reader["FECHA"];
                                        double unidades = reader.GetDouble(1);

                                        invcartones.Add(new PinventarioModel()
                                        {
                                            fecha = fecha,
                                            unidades = unidades,
                                        });
                                    }
                                }
                            }

                            if (invcartones.Count > 0)
                            {
                                cartonescapturados = true;
                                cartones = invcartones[0].unidades;
                            }
                        }

                        Boolean tienedescuento = false;
                        var regdesc = _dbpContext.Descuentos.Where(x => x.Codprov == item.Codproveedor).FirstOrDefault();
                        if (regdesc != null) { tienedescuento = true; }

                        pedidos.Add(new Pedidos()
                        {
                            idSucursal = item.Codsucursal.ToString()
                           ,
                            codProveedor = item.Codproveedor,
                            total = totalpedido,
                            fechaEntrega = fechaentrega,
                            articulos = articulospedido,
                            nombreproveedor = nombreproveedor,
                            nombresucursal = nombresucursal,
                            status = status,
                            rfc = rfcprov,
                            cartones = cartones,
                            tieneretornables = requierecartones,
                            capturacartones = cartonescapturados,
                            tienedescuento = tienedescuento,
                            cantidaddescuento = 0
                        });

                        string tempjdata = JsonConvert.SerializeObject(pedidos.Last());
                        var temppedido = _dbpContext.Pedidos.Where(p => p.Sucursal == item.Codsucursal.ToString() && p.Proveedor == item.Codproveedor && p.Jdata == tempjdata && p.Fecha.Value.Date == DateTime.Now.Date).FirstOrDefault();
                        if (temppedido == null)
                        {
                            await _dbpContext.Pedidos.AddAsync(new Pedido()
                            {
                                Sucursal = item.Codsucursal.ToString(),
                                Proveedor = item.Codproveedor,
                                Jdata = tempjdata,
                                Estatus = status == 1 ? "POR ACEPTAR" : "INCOMPLETO",
                                Fecha = DateTime.Now,
                                Numpedido = "",
                                Idcal = item.Id,
                                Temporal = false
                            });
                            await _dbpContext.SaveChangesAsync();
                        }
                        else
                        {
                            pedidos.RemoveAt(pedidos.Count - 1);
                        }

                    }

                }

                var estatuspedido = _dbpContext.ValidacionPedidos.Where(x => x.Idu == idu).FirstOrDefault();
                if (estatuspedido != null)
                {
                    _dbpContext.ValidacionPedidos.Remove(estatuspedido);
                    await _dbpContext.SaveChangesAsync();
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


    }

    public class AceptarTodoModel 
    {
        public int proveedor {  get; set; }
        public int sucursal { get; set; }
        public int idu { get; set; }
        public DateTime? fecha { get; set; }
        
    }
    public class InsertCompratotModel
    {
        public string numserie { get; set; }
        public int numpedido { get; set; }
        public int numlinea { get; set; }
        public double totbruto { get; set; }
        public double iva { get; set; }
        public double totaliva { get; set; }
        public double ieps { get; set; }
        public double totalieps { get; set; }
        public double total { get; set; }

    }

    public class InsertPedidoModel 
    {
        public string numserie { get; set; }
        public int numpedido { get; set; }
        public int codproveedor { get; set; }
        public DateTime fechapedido { get; set; }
        public DateTime fechaentrega { get; set; } 
        public double totbruto { get; set; }
        public double totimpuestos { get; set; }
        public int tipoimpusto { get; set; }
        public double totneto { get; set; }

        public string supedido { get; set; }

    }

    public class InsertPedidoLinModel
    {

        public string numserie { get; set; }
        public int numpedido { get; set; }
        public int numlinea { get; set; }   
        public int codarticulo { get; set; }
        public string referencia { get; set; } 
        public string descripcion { get; set; }
        public double cajas { get; set; }
        public double unidades { get; set; }
        public double unidadestotales { get; set; }
        public double precio { get; set; }
        public int tipoimpuesto { get; set; }
        public double iva { get; set; }
        public double ieps { get; set; }
        public double total { get; set;}
        public string codalmacen { get; set; }
        public string supedido { get; set; }
    }

    public class ModificacionesModel
        {
         public int id { get; set; }    
         public string justificacion { get; set; }
        public double inventario { get; set; }  
        public int codarticulo { get; set; }
        public int idusuario { get; set; }  
    }

    public class AjusteComprasModel
    {
        public int id { get; set; }
        public string justificacion { get; set; }
        public double unidades { get; set; }
        public int cajas { get; set; }
        public int idusuario {  get; set; }
        public int codarticulo { get; set; }
        public string comentario { get; set; }
    }
    public class umedidaModel
    {
        public string rfc { get; set; }
        public string noIdentificacion { get; set; }
        public int codarticulo { get; set; }    
        public string umedida { get; set; } 
        public decimal? uds { get; set; } 
        public string nomprov { get; set; }
        public string nomarticulo { get; set; } 
    }

    public class itproductoModel
    {
        public string rfc {  get; set; }
        public int codarticulo {  get; set; }
        public string umedida { get; set; }
        public decimal uds {  get; set; }

    }

    public class PinventarioModel 
    {
        public DateTime fecha { get; set; }
        public double unidades { get; set; }
    }

    public class Pedidos 
    { 
        public int id { get; set; }
        public string idSucursal { get; set; }
        public string nombresucursal { get; set; }  
        public string nombreproveedor { get; set; } 
        public int codProveedor { get; set; }   
        public DateTime fechaEntrega { get; set;}

        public double total { get; set; }
        public int status { get; set; } 

        public string rfc { get; set; }
        public List<ArticuloPedido> articulos { get; set; }

        public Boolean tieneretornables { get; set; }
        public double cartones { get; set; }
        public Boolean capturacartones { get; set; }

        public Boolean tienedescuento { get; set; }
        public double cantidaddescuento { get; set; }
    }


    public class PedidosH
    {
        public int id { get; set; }
        public string idSucursal { get; set; }
        public string nombresucursal { get; set; }
        public string nombreproveedor { get; set; }
        public int codProveedor { get; set; }
        public DateTime fechaEntrega { get; set; }
        public DateTime fechapedido { get; set; }   

        public double total { get; set; }
        public int status { get; set; }

        public string rfc { get; set; }
        public List<ArticuloPedido> articulos { get; set; }

        public string? numpedido {  get; set; }

        public bool notificado { get; set; }    

    }


    public class ArticuloPedido
    {
        public int codArticulo { get; set; }
        public string nombre { get; set; }
        public double inventariohoy { get; set; }
        public double? precio { get; set; }
        public int numlinea { get; set; }

        public int cajas { get; set; }
        public double unidadescaja { get; set; }
        public double unidadestotales { get; set; }
        public int tipoImpuesto { get; set; }

        public double iva { get; set; }
        public double total_linea { get; set; }
        public string codigoAlmacen { get; set; }

        public Boolean tienemultiplo { get; set; }
        public Boolean hayinventario { get; set; }

        public List<ConsumoModel> consumospromedios { get; set; }
        public double consumomayor { get; set; }
        public double factorseguridad { get; set; }

        public int[] arraycalendario { get; set; }  
        public DiasEspecialesSucursal[] diasespeciales { get; set; }

        public Boolean calendarioespecial { get; set; }
        public double unidadesextra { get; set; }

        public Boolean esretornable { get; set; }

        public Boolean tienelimitealmacen { get; set; }
        public double capacidadalmfinal { get; set; }

        public Boolean invformulado {  get; set; }

    }

    public class articuloModel 
    {
        public int cod { get; set; }
        public string descripcion { get; set; }
        public double precio { get; set; }
        public ICollection<Referenciasprov> referencia { get; set; }
        public int tipoimpuesto { get; set; }
    }


}

