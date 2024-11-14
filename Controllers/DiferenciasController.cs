using API_PEDIDOS.ModelsBD2P;
using API_PEDIDOS.ModelsDB2;
using API_PEDIDOS.ModelsDBP;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using System.Globalization;

namespace API_PEDIDOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiferenciasController : ControllerBase
    {
        BD2Context _db2Context;
        private readonly IConfiguration _configuration;
        public string connectionString = string.Empty;
        public string connectionStringDBREBEL = string.Empty;
        public string connectionStringBd2 = string.Empty;
        protected DBPContext _dbpContext;
        protected BD2PContext _bd2pcontext; 


        public DiferenciasController(BD2Context bD2Context, IConfiguration configuration, DBPContext dBPContext, BD2PContext bD2PContext)
        {
            _db2Context = bD2Context;
            _dbpContext = dBPContext;
            _bd2pcontext = bD2PContext;
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection");
            connectionStringBd2 = _configuration.GetConnectionString("DB2");
        }


        [HttpPost("getDiferencias")]
        public List<Reporte> GetReporteV([FromForm] DateTime fecha)
        {
            List<Reporte> reportes = new List<Reporte>();
            SqlConnection connection = (SqlConnection)_dbpContext.Database.GetDbConnection();
            SqlCommand cmd = connection.CreateCommand();
            connection.Open();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "SPS_INV_VESP_REPORTE";
            cmd.Parameters.Add("@FECHA", System.Data.SqlDbType.VarChar, 10).Value = fecha.ToString("dd/MM/yyyy");
            cmd.CommandTimeout = 120;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Reporte repp = new Reporte();
                repp.cod = (string)reader["COD"];
                repp.Region = (string)reader["REGION"];
                repp.Sucursal = (string)reader["SUCURSAL"];
                repp.Articulo = (string)reader["ARTICULO"];
                repp.Seccion = (string)reader["SECCION"];
                repp.InvAyer = (string)reader["INVAYER"];
                repp.ConsumoAyer = (double)reader["CONSUMOAYER"];
                repp.TraspasoAyer = (double)reader["TRASPASOAYER"];
                repp.InvHoy = (string)reader["INVHOY"];
                repp.Captura = (DateTime)reader["CAPTURA"];
                repp.InvFormula = (double)reader["INVFORMULA"];
                repp.Diferencia = (double)reader["DIFERENCIA"];
                repp.Mermasayer = (double)reader["MERMASAYER"];
                repp.codart = (int)reader["CODART"]; 
                reportes.Add(repp);
            }
            connection.Close();

            return reportes;
        }

        [HttpPost("getDiferenciaLin")]
        public List<Reporte> GetDiferenciaLin([FromForm] DateTime fecha, [FromForm] string codalm, [FromForm] int codart)
        {
            List<Reporte> reportes = new List<Reporte>();
            //SqlConnection connection = (SqlConnection)_db2Context.Database.GetDbConnection();
            SqlConnection connection = (SqlConnection)_dbpContext.Database.GetDbConnection();
            SqlCommand cmd = connection.CreateCommand();
            connection.Open();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "SPS_GET_DIFERENCIA_LIN";
            cmd.Parameters.Add("@FECHA", System.Data.SqlDbType.VarChar, 10).Value = fecha.ToString("dd/MM/yyyy");
            cmd.Parameters.Add("@CODALM", System.Data.SqlDbType.NVarChar, 10).Value = codalm;
            cmd.Parameters.Add("@CODART", System.Data.SqlDbType.Int).Value = codart;
            cmd.CommandTimeout = 120;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Reporte repp = new Reporte();
                repp.cod = (string)reader["COD"];
                repp.Region = (string)reader["REGION"];
                repp.Sucursal = (string)reader["SUCURSAL"];
                repp.Articulo = (string)reader["ARTICULO"];
                repp.Seccion = (string)reader["SECCION"];
                repp.InvAyer = (string)reader["INVAYER"];
                repp.ConsumoAyer = (double)reader["CONSUMOAYER"];
                repp.TraspasoAyer = (double)reader["TRASPASOAYER"];
                repp.InvHoy = (string)reader["INVHOY"];
                repp.Captura = (DateTime)reader["CAPTURA"];
                repp.InvFormula = (double)reader["INVFORMULA"];
                repp.Diferencia = (double)reader["DIFERENCIA"];
                repp.Mermasayer = (double)reader["MERMASAYER"];
                repp.codart = (int)reader["CODART"];
                reportes.Add(repp);
            }
            connection.Close();

            return reportes;
        }


        //[HttpPost("getMermasSucursal")]
        //public List<MermaSucursal> GetMermasSucursal([FromForm] DateTime fecha, [FromForm] string sucursal, [FromForm] int codart)
        //{
        //    List<MermaSucursal> mermasList = new List<MermaSucursal>();
        //    DateTime fechasp = fecha.AddDays(-1);
        //    try
        //    {
        //        using (SqlConnection connection = new SqlConnection(connectionString))
        //        {
        //            using (SqlCommand command = new SqlCommand("SP_GET_MERMAS_SUC", connection))
        //            {
        //                command.CommandType = CommandType.StoredProcedure;

        //                // Agregar parámetros
        //                command.Parameters.AddWithValue("@sucursal", sucursal);
        //                command.Parameters.AddWithValue("@articulo", codart);
        //                command.Parameters.AddWithValue("@FECHA", fechasp.ToString("yyyy-MM-dd"));

        //                // Abrir conexión
        //                connection.Open();

        //                // Ejecutar el comando y usar SqlDataReader para leer los resultados
        //                using (SqlDataReader reader = command.ExecuteReader())
        //                {
        //                    while (reader.Read())
        //                    {
        //                        MermaSucursal merma = new MermaSucursal
        //                        {
        //                            ID = (int)reader["ID"],
        //                            CodAlmacen = (string)reader["CODALMACEN"],
        //                            CodArticulo = (int)reader["CODARTICULO"],
        //                            Precio = (double)reader["PRECIO"],
        //                            fecha = (DateTime)reader["FECHA"],
        //                             Unidades = (Double)reader["UNIDADES"]
        //                        };

        //                        mermasList.Add(merma);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Error: " + ex.Message);
        //    }

        //    return mermasList;
        //}


        [HttpPost]
        [Route("AddInv")]
        public async Task<ActionResult> addInv([FromForm] string codalm, [FromForm] double unidades, [FromForm] int codart ,[FromForm] int idu, [FromForm] DateTime fecha)
        {
            try
            {
             
                var diAsignado = fecha.Date;

                var __stock = _db2Context.Stocks.FirstOrDefault(x => x.Codarticulo == codart && x.Codalmacen == codalm);
                double _stockAnterior = __stock.Stock1.Value;
                if (__stock != null)
                {
                    if (fecha.Date == DateTime.Now.Date) 
                    {
                        __stock.Stock1 = unidades;

                        _db2Context.Stocks.Update(__stock);
                    }
                      
                    DateTime FechVC = diAsignado.Date.AddHours(2);
                    double precio = 0;

                    try { precio = (double)(_db2Context.Articuloscamposlibres.FirstOrDefault(x => x.Codarticulo == codart)?.Precioproveedor); } catch (Exception ex) { }

                    ModelsDB2.Moviment _moviment = new ModelsDB2.Moviment();
                    _moviment.Codalmacenorigen = codalm;
                    _moviment.Codalmacendestino = "";
                    _moviment.Numserie = "";
                    _moviment.Codarticulo = codart;
                    _moviment.Talla = ".";
                    _moviment.Color = ".";
                    _moviment.Precio = precio;
                    _moviment.Fecha = FechVC.Date;
                    _moviment.Hora = Convert.ToDateTime("1899-12-30 " + FechVC.Hour + ":" + FechVC.Minute + ":" + FechVC.Second + ".000");
                    _moviment.Codprocli = 0;
                    _moviment.Tipo = "REG";
                    _moviment.Unidades = unidades;
                    _moviment.Seriedoc = "";
                    _moviment.Numdoc = 0;
                    _moviment.Seriecompra = "";
                    _moviment.Numfaccompra = -1;
                    _moviment.Caja = "";
                    _moviment.Stock = _stockAnterior;
                    _moviment.Pvp = 0;
                    _moviment.Codmonedapvp = 1;
                    _moviment.Calcmovpost = "F";
                    _moviment.Udmedida2 = 0;
                    _moviment.Zona = "";
                    _moviment.Pvpdmn = null;
                    _moviment.Preciodmn = null;
                    _moviment.Stock2 = 0;

                    _db2Context.Moviments.Add(_moviment);

                    _db2Context.SaveChanges();
                }

                // AGREGAR REGISTRO AL LOG DE MODIFICACIONES

                _dbpContext.LogDiferencias.Add(new LogDiferencia()
                {
                    Tipo = "INVENTARIO",
                    ValAntes = "SIN CAPTURA",
                    ValDespues = unidades.ToString(),
                    Fecha = DateTime.Now,
                    Idu = idu,
                    Codalm = codalm,
                    Codart = codart,
                    Justificacion = "ERROR EN INVENTARIO"
                });
                await _dbpContext.SaveChangesAsync();


                return StatusCode(200);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.ToString() });
            }


        }

        [HttpPost]
        [Route("UpdateUnidades")]
        public async Task<ActionResult> updateUnidades([FromForm] int id, [FromForm] double unidades, [FromForm] int idu)
        {
            try
            {
                var regmoviment = _db2Context.Moviments.Where(x => x.Id == id).FirstOrDefault(); 
                
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("SP_UPDATE_INVENTARIO", connection))
                    {
                        // Especificar que se trata de un procedimiento almacenado
                        command.CommandType = CommandType.StoredProcedure;

                        // Agregar los parámetros con los valores correspondientes
                        command.Parameters.Add(new SqlParameter("@UNIDADES", SqlDbType.Float) { Value = unidades });
                        command.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int) { Value = id });

                        // Ejecutar el comando
                        int rowsAffected = command.ExecuteNonQuery();
                    }
                }

                // actualizar stock 

                if (regmoviment.Fecha.Value.Date == DateTime.Now.Date) 
                {
                    var __stock = _db2Context.Stocks.FirstOrDefault(x => x.Codarticulo == regmoviment.Codarticulo && x.Codalmacen == regmoviment.Codalmacenorigen);
                    __stock.Stock1 = unidades;

                    _db2Context.Stocks.Update(__stock);

                    await _db2Context.SaveChangesAsync();
                }
       
                // AGREGAR REGISTRO AL LOG DE MODIFICACIONES

                _dbpContext.LogDiferencias.Add(new LogDiferencia()
                {
                    Tipo = "INVENTARIO",
                    ValAntes = regmoviment.Unidades.ToString(),
                    ValDespues = unidades.ToString(),
                    Fecha = DateTime.Now,
                    Idu = idu,
                    Codalm = regmoviment.Codalmacenorigen,
                    Codart = regmoviment.Codarticulo,   
                    Justificacion = "ERROR EN INVENTARIO"
                });
                await _dbpContext.SaveChangesAsync();


                return StatusCode(200); 
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.ToString() });
            }


        }

        [HttpPost]
        [Route("getLineaInv")]
        public async Task<ActionResult> getlineainv([FromForm] string sucursal, [FromForm] int articulo,[FromForm] DateTime fecha)
        {
            try
            {
                int id = -1;
                double unidades = -1; 
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("SP_GET_LINEA_INVENTARIO", connection))
                    {
                        // Especificar que se trata de un procedimiento almacenado
                        command.CommandType = CommandType.StoredProcedure;

                        // Agregar los parámetros con los valores correspondientes
                        command.Parameters.Add(new SqlParameter("@sucursal", SqlDbType.NVarChar, 5) { Value = sucursal });
                        command.Parameters.Add(new SqlParameter("@articulo", SqlDbType.Int) { Value = articulo });
                        command.Parameters.Add(new SqlParameter("@FI", SqlDbType.NVarChar, 255) { Value = fecha.ToString("yyyy-MM-dd") });

                        // Ejecutar el comando y leer los resultados
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Leer las columnas según su tipo
                                 id = (int)reader["ID"];
                                 unidades = (double)reader["UNIDADES"];
                            }
                        }
                    }
                }

                return StatusCode(StatusCodes.Status200OK, new { id= id, unidades = unidades });

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.ToString() });
            }


        }


        [HttpPost("updateMermas")]
        public async Task<ActionResult> insertartraspaso([FromBody] TraspasoCabModel model)
        {

            try
            {
                model.fecha = model.fecha.AddDays(-1); 
                string serie = _db2Context.Almacens.Where(x => x.Codalmacen == model.codalmacen + "M").FirstOrDefault().Serietraspasos;
                int numtrasp = 0;
                var seriedb = _db2Context.Series.Where(x => x.Serie == serie).FirstOrDefault();
                if (seriedb != null) { numtrasp = (int)seriedb.Numtrasp;}
                numtrasp++;
                double precio = 0;

                try { precio = (double)(_db2Context.Articuloscamposlibres.FirstOrDefault(x => x.Codarticulo == model.codarticulo)?.Precioproveedor); } catch(Exception ex) { }

                double cantidad = 0;

                cantidad = model.nuevacantidad - model.cantidadanterior;

                if (cantidad != 0) 
                {
                    ModelsDB2.Traspasoscab tcm = new ModelsDB2.Traspasoscab();
                    tcm.Serie = serie;
                    tcm.Numero = numtrasp;
                    tcm.Caja = model.codalmacen + "1";
                    tcm.Fecha = model.fecha;
                    tcm.Codalmacenorigen = model.codalmacen;
                    tcm.Codalmacendestino = model.codalmacen + "M";
                    tcm.Numfaccompra = 0;
                    tcm.Total = cantidad * precio;
                    tcm.Anulado = "F";
                    tcm.Serieanulado = "";
                    tcm.Cajaanulado = "";
                    tcm.Numeroanulado = 0;
                    tcm.Recibido = "T";
                    tcm.Fecharecibido = model.fecha;
                    tcm.Descargado = "T";
                    tcm.Observaciones = "";
                    tcm.Esautomatico = "F";
                    tcm.Esrecuento = "F";
                    tcm.Esajuste = "F";
                    tcm.Escontabilizable = "T";
                    tcm.Fechacreacion = DateTime.Now;
                    tcm.Impresiones = 1;
                    tcm.Modificable = "F";

                    _db2Context.Traspasoscabs.Add(tcm);


                    var __stock = _db2Context.Stocks.FirstOrDefault(x => x.Codarticulo == model.codarticulo && x.Codalmacen == model.codalmacen);
                    double _stockAnterior = __stock.Stock1.Value;


                    DateTime FechVC = model.fecha.Date.AddHours(2);
                    ModelsDB2.Moviment _moviment = new ModelsDB2.Moviment();
                    _moviment.Codalmacenorigen = model.codalmacen;
                    _moviment.Codalmacendestino = model.codalmacen + "M";
                    _moviment.Numserie = null;
                    _moviment.Codarticulo = model.codarticulo;
                    _moviment.Talla = ".";
                    _moviment.Color = ".";
                    _moviment.Precio = precio;
                    _moviment.Fecha = FechVC.Date;
                    _moviment.Hora = Convert.ToDateTime("1899-12-30 " + FechVC.Hour + ":" + FechVC.Minute + ":" + FechVC.Second + ".000");
                    _moviment.Codprocli = 0;
                    _moviment.Tipo = "ENV";
                    _moviment.Unidades = cantidad;
                    _moviment.Seriedoc = serie;
                    _moviment.Numdoc = numtrasp;
                    _moviment.Seriecompra = null;
                    _moviment.Numfaccompra = -1;
                    _moviment.Caja = model.codalmacen + "1";
                    _moviment.Stock = _stockAnterior;
                    _moviment.Pvp = 0;
                    _moviment.Codmonedapvp = 1;
                    _moviment.Calcmovpost = "F";
                    _moviment.Udmedida2 = 0;
                    _moviment.Zona = null;
                    _moviment.Pvpdmn = null;
                    _moviment.Preciodmn = null;
                    _moviment.Stock2 = null;

                    _db2Context.Moviments.Add(_moviment);

                    // actualizar stock 

                    //__stock.Stock1 = (_stockAnterior-cantidad);
                    //_db2Context.Stocks.Update(__stock);

                    // actualizar series 

                    var regserie = _db2Context.Series.Where(x => x.Serie == serie).FirstOrDefault();
                    if (regserie != null)
                    {
                        regserie.Numtrasp = numtrasp;
                        _db2Context.Series.Update(regserie);
                    }


                    await _db2Context.SaveChangesAsync();


                    // AGREGAR REGISTRO AL LOG DE MODIFICACIONES

                    _dbpContext.LogDiferencias.Add(new LogDiferencia()
                    {
                        Tipo = "MERMA",
                        ValAntes = model.cantidadanterior.ToString(),
                        ValDespues = model.nuevacantidad.ToString(),
                        Fecha = DateTime.Now,
                        Idu = model.idu,
                        Justificacion = "ERROR EN MERMAS",
                        Codalm = model.codalmacen,
                        Codart = model.codarticulo
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



    }

    public class Reporte
    {
        public string cod { get; set; }
        public string Region { get; set; }
        public string Sucursal { get; set; }
        public string Articulo { get; set; }
        public string InvAyer { get; set; }
        public double TraspasoAyer { get; set; }
        public double ConsumoAyer { get; set; }
        public string InvHoy { get; set; }
        public double InvFormula { get; set; }
        public double Diferencia { get; set; }
        public DateTime Captura { get; set; }
        public string Seccion { get; set; }
        public double? Mermasayer { get; set; }
        public int codart { get; set; }
    }

    public class MermaSucursal
    {
        public int ID { get; set; }
        public string CodAlmacen { get; set; }
        public int CodArticulo { get; set; }
        public double Precio { get; set; }
        public DateTime fecha { get; set; }
        public double Unidades { get; set; }

    }


    public class TraspasoCabModel
    {
        public string codalmacen { get; set; }

        public int codarticulo { get; set; }
        public double cantidadanterior { get; set; }
        public double nuevacantidad { get; set; }
        public DateTime fecha { get; set; }

        public int idu { get; set; }
    }


}
