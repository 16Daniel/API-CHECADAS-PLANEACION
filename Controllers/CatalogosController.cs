using API_PEDIDOS.ModelsDB2;
using API_PEDIDOS.ModelsDBP;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Data;
using System.Globalization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace API_PEDIDOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class CatalogosController : ControllerBase
    {
        private readonly ILogger<CatalogosController> _logger;
        protected BD2Context _contextdb2;
        protected DBPContext _dbpContext;

        public CatalogosController(ILogger<CatalogosController> logger, BD2Context db2c, DBPContext dbpc) 
        {
            _logger = logger;
            _contextdb2 = db2c;
            _dbpContext = dbpc;
        }

        [HttpGet]
        [Route("getProveedores")]
        public async Task<ActionResult> GetProveedores()
        {
            try
            {
                //var repository = _contextdb2.Proveedores.Where(p => p.Descatalogado =="F").Select(s => new
                //{
                //    codproveedor = s.Codproveedor,
                //    nombre = s.Nomproveedor,
                //}).ToList();

                var query = from prov in _contextdb2.Proveedores
                        join provcl in _contextdb2.Proveedorescamposlibres
                        on prov.Codproveedor equals provcl.Codproveedor into gj
                        from subprov in gj.DefaultIfEmpty()
                        where subprov != null && subprov.Planeacion == "T"
                        select new
                        {
                            codproveedor = prov.Codproveedor,
                            nombre = prov.Nomproveedor,
                            rfc = prov.Cif
                        };

                return StatusCode(200,query.ToList());
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
        [Route("getSucursales")]
        public async Task<ActionResult> GetSucursales()
        {
            try
            {
                var query = from rf in _contextdb2.RemFronts
                            where rf.Descatalogado == false
                            select new
                            {
                                cod = rf.Idfront,
                                name = rf.Titulo
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
        [Route("getItemsprov")]
        public async Task<ActionResult> GetItemsprov()
        {
            try
            {
            
                var query = from art in _contextdb2.Articulos1
                            join artcl in _contextdb2.Articuloscamposlibres
                            on art.Codarticulo equals artcl.Codarticulo into gj
                            from subartcl in gj.DefaultIfEmpty()
                            where subartcl != null && subartcl.Planeacion == "T"
                            select new
                            {
                                cod = art.Codarticulo,
                                descripcion = art.Descripcion,
                                marca = art.Marca
                            };

                return StatusCode(200,query.ToList());
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
        [Route("getItemsprovPlaneacion/{idprov}")]
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
                            where subartcl != null && subartcl.Planeacion == "T" && subprec.Codproveedor == idprov
                            select new
                            {
                                cod = art.Codarticulo,
                                descripcion = art.Descripcion,
                                marca = art.Marca,
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
        [Route("getItemsCal/{idprov}/{ids}")]
        public async Task<ActionResult> GetItemsCal(int idprov, int ids)
        {
            try
            {
                var articulosdb = _dbpContext.ArticulosProveedors.Where(x => x.Codsucursal == ids && x.Codprov == idprov).ToList();
                List<Object> articulos = new List<Object>();  
                foreach (var item in articulosdb) 
                {
                    var art = _contextdb2.Articulos1.Where(x => x.Codarticulo == item.Codarticulo).FirstOrDefault();
                    articulos.Add(new 
                    {
                        cod = art.Codarticulo,
                        descripcion = art.Descripcion,
                        marca = art.Marca,  
                    });
                }

                return StatusCode(200, articulos);
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
        [Route("getConsumo/{sucursal}/{articulo}/{semanas}/{fecha}")]
        public async Task<ActionResult> GetConsumo(int sucursal,int articulo, int semanas, string fecha)
        {
            try
            {
                List<ConsumoModel> consumos = new List<ConsumoModel>(); 
                // Crear conexión
                using (SqlConnection connection = (SqlConnection)_dbpContext.Database.GetDbConnection())
                {
                    connection.Open();

                    // Crear comando para ejecutar el procedimiento almacenado
                    using (SqlCommand command = new SqlCommand("SP_Consumo_Promedio2", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Agregar parámetros al procedimiento almacenado
                        command.Parameters.Add("@sucursal", SqlDbType.NVarChar).Value = sucursal;
                        command.Parameters.Add("@articulo", SqlDbType.Int).Value = articulo;
                        command.Parameters.Add("@semanas", SqlDbType.Int).Value = semanas;
                        command.Parameters.Add("@fecha", SqlDbType.DateTime).Value = DateTime.ParseExact(fecha, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                        try
                        {
                            // Ejecutar el procedimiento almacenado
                            SqlDataReader reader = command.ExecuteReader();

                            while (reader.Read())
                            {
                                consumos.Add(new ConsumoModel 
                                { dia=int.Parse(reader["DIA"].ToString()),
                                  consumo = double.Parse(reader["CONSUMO"].ToString()) });
                            }

                            reader.Close();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error al ejecutar el procedimiento almacenado: " + ex.Message);
                        }
                    }
                }

                return StatusCode(200,consumos);
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

    public class ConsumoModel 
    {
        public int dia { get; set; }
        public double consumo { get; set; }
    }

 
}
