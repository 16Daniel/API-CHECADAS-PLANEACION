using API_PEDIDOS.ModelsDB2;
using API_PEDIDOS.ModelsDBP;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Data;
using System.Drawing;

namespace API_PEDIDOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChecadasController : ControllerBase
    {
        private readonly ILogger<CatalogosController> _logger;
        protected BD2Context _contextdb2;
        protected DBPContext _dbpContext;

        public ChecadasController(ILogger<CatalogosController> logger, BD2Context db2c, DBPContext dbpc)
        {
            _logger = logger;
            _contextdb2 = db2c;
            _dbpContext = dbpc;
        }

        [HttpGet]
        [Route("getUbicaciones")]
        public async Task<ActionResult> GetUbicaciones()
        {
            try
            {
                List<CatModel> ubicaciones = new List<CatModel>();
                // Crear conexión
                using (SqlConnection connection = (SqlConnection)_dbpContext.Database.GetDbConnection())
                {
                    connection.Open();

                    // Crear comando para ejecutar el procedimiento almacenado
                    using (SqlCommand command = new SqlCommand("SP_CHECADAS_UBICACIONES", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        try
                        {
                            // Ejecutar el procedimiento almacenado
                            SqlDataReader reader = command.ExecuteReader();

                            while (reader.Read())
                            {
                                ubicaciones.Add(new CatModel
                                {
                                    id = int.Parse(reader["CLA_RELOJ"].ToString()),
                                    name = reader["NOM_RELOJ"].ToString()
                                });
                            }

                            reader.Close();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error al ejecutar el procedimiento almacenado: " + ex.Message);
                        }
                    }
                }

                return StatusCode(200, ubicaciones);
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
        [Route("getDepartamentos")]
        public async Task<ActionResult> Getdepartamentos()
        {
            try
            {
                List<CatModel> departamentos = new List<CatModel>();
                // Crear conexión
                using (SqlConnection connection = (SqlConnection)_dbpContext.Database.GetDbConnection())
                {
                    connection.Open();

                    // Crear comando para ejecutar el procedimiento almacenado
                    using (SqlCommand command = new SqlCommand("SP_CHECADAS_DEPARTAMENTOS", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        try
                        {
                            // Ejecutar el procedimiento almacenado
                            SqlDataReader reader = command.ExecuteReader();

                            while (reader.Read())
                            {
                                departamentos.Add(new CatModel
                                {
                                    id = int.Parse(reader["CLA_PUESTO"].ToString()),
                                    name = reader["NOM_PUESTO"].ToString()
                                });
                            }

                            reader.Close();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error al ejecutar el procedimiento almacenado: " + ex.Message);
                        }
                    }
                }

                return StatusCode(200, departamentos);
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
        [Route("getEmpleados/{id}")]
        public async Task<ActionResult> GetEmpleados(int id)
        {
            try
            {
                List<EmpleadoModel> empleados = new List<EmpleadoModel>();
                // Crear conexión
                using (SqlConnection connection = (SqlConnection)_dbpContext.Database.GetDbConnection())
                {
                    connection.Open();

                    // Crear comando para ejecutar el procedimiento almacenado
                    using (SqlCommand command = new SqlCommand("SP_CHECADAS_EMPLEADOS", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@CLA_DEPTO", SqlDbType.Int).Value = id;
                        try
                        {
                            // Ejecutar el procedimiento almacenado
                            SqlDataReader reader = command.ExecuteReader();

                            while (reader.Read())
                            {
                                empleados.Add(new EmpleadoModel
                                {
                                    id = int.Parse(reader["CLA_TRAB"].ToString()),
                                    nombre = reader["NOM_TRAB"].ToString(),
                                    apellidop = reader["AP_PATERNO"].ToString(),
                                    apellidom = reader["AP_MATERNO"].ToString()
                                });
                            }

                            reader.Close();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error al ejecutar el procedimiento almacenado: " + ex.Message);
                        }
                    }
                }

                return StatusCode(200, empleados);
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
        [Route("saveCalendariosChecadas")]
        public async Task<ActionResult> savecalendarioschecadas(CalendariosChecada model)
        {
            try
            {
                var calendario = _dbpContext.CalendariosChecadas.Where(c => c.IdPuesto == model.IdPuesto && c.IdEmpleado == model.IdEmpleado ).FirstOrDefault();
                if (calendario == null)
                {
                    _dbpContext.CalendariosChecadas.Add(model);
                    await _dbpContext.SaveChangesAsync();
                }
                else 
                {
                    calendario.Jdata = model.Jdata; 
                    _dbpContext.Entry(calendario).State = EntityState.Modified;
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
        [Route("getCalendarioChecada/{idpuesto}/{idempleado}")]
        public async Task<ActionResult> Getcalendariochecada(int idpuesto, int idempleado)
        {
            try
            {
                CalendariosChecada calendario = _dbpContext.CalendariosChecadas.Where(c=> c.IdPuesto == idpuesto && c.IdEmpleado == idempleado).FirstOrDefault();
                if (calendario != null)
                {
                    return StatusCode(200, calendario);
                }
                else 
                {
                    return StatusCode(StatusCodes.Status404NotFound);
                }
               
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
        [Route("deleteCalendarioChecadas/{id}")]
        public async Task<ActionResult> deletecalendaario(int id)
        {
            try
            {
                CalendariosChecada calendario = _dbpContext.CalendariosChecadas.Find(id); 
                if (calendario != null)
                {
                    _dbpContext.CalendariosChecadas.Remove(calendario);
                    await _dbpContext.SaveChangesAsync();
                    return StatusCode(200);
                }
                else
                {
                    return StatusCode(StatusCodes.Status404NotFound);
                }

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
        [Route("getCatStatusChecadas")]
        public async Task<ActionResult> getstatuschecadas()
        {
            try
            {
                var catalogo = _dbpContext.CatStatusChecadas.ToList();
                return StatusCode(StatusCodes.Status200OK, catalogo);
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
        [Route("getCatStatusChecadas/{iduser}/{fi}/{ff}")]
        public async Task<ActionResult> getChecadasbyUser(int iduser, string fi, string ff)
        {
            try
            {
                List<checadasModel> checadas = new List<checadasModel>(); 

                using (SqlConnection connection = (SqlConnection)_dbpContext.Database.GetDbConnection())
                {
                    // Crear el comando para ejecutar el procedimiento almacenado
                    SqlCommand command = new SqlCommand("SP_CHECADAS_getChecadas", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    // Parámetros del procedimiento almacenado
                    command.Parameters.AddWithValue("@fi", fi);
                    command.Parameters.AddWithValue("@ff", ff);
                    command.Parameters.AddWithValue("@idu", iduser);

                    // Abrir la conexión
                    connection.Open();

                        // Ejecutar el comando
                        SqlDataReader reader = command.ExecuteReader();

                        // Leer los resultados del primer conjunto de resultados
                        while (reader.Read())
                        {

                        string nomreloj = "";
                        try { nomreloj = (string)reader["NOM_RELOJ"];  } catch { }   

                        checadas.Add(
                         new checadasModel() 
                         {
                             fecha = (DateTime)reader["FECHA_ENTSAL"],
                             cla_trab = (int)reader["CLA_TRAB"],
                             cla_reloj = (int)reader["CLA_RELOJ"],
                             nom_reloj = nomreloj,
                             nombre = (string)reader["NOM_TRAB"].ToString().Trim(),
                             ap_paterno = (string)reader["AP_PATERNO"].ToString().Trim(),
                             ap_materno = (string)reader["AP_MATERNO"].ToString().Trim(),
                         }   
                         );
                                
                        }

                        // Cerrar el lector de datos
                        reader.Close();
                }
                return StatusCode(StatusCodes.Status200OK,checadas);
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
        [Route("generateExcel")]
        public IActionResult GenerateExcel(ExcelModel model)
        {
            string fechai = model.fi.ToString("dd/MM/yyyy");
            string fechaf = model.ff.ToString("dd/MM/yyyy"); 
            string[][] data = JsonConvert.DeserializeObject<string[][]>(model.jdata);
            Color colorcelda = ColorTranslator.FromHtml("#00000000");
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            // Crear un nuevo archivo de Excel
            using (var package = new ExcelPackage())
            {
                // Agregar una hoja al libro de trabajo
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                worksheet.Cells[1, 1].Value = "FECHA INICIAL";
                worksheet.Cells[1, 2].Value = "FECHA FINAL";
                using (var range = worksheet.Cells["A1:B1"])
                {
                    Color colorFondo = ColorTranslator.FromHtml("#00000000");
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(colorFondo);
                    range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    range.AutoFitColumns();
                }

                worksheet.Cells[2, 1].Value = fechai;
                worksheet.Cells[2, 2].Value = fechaf;
                worksheet.Cells[3, 1].Value = "";
                worksheet.Cells[3, 2].Value = "";


                for (int i = 0; i<data.Length; i++) 
                {
                    for (int j = 0; j <= 9; j++) 
                    {
                        worksheet.Cells[i+4,j+1].Value = data[i][j];
                        colorcelda = ColorTranslator.FromHtml(data[i][10]);
                        worksheet.Cells[i + 4, j + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[i + 4, j + 1].Style.Fill.BackgroundColor.SetColor(colorcelda);
                        worksheet.Cells[i + 4, j + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }
                }
                // Establecer el estilo de las celdas
                using (var range = worksheet.Cells["A4:J4"])
                {
                    Color colorFondo = ColorTranslator.FromHtml("#00000000");
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(colorFondo);
                    range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    range.AutoFitColumns(); 
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


        [HttpGet]
        [Route("getEmpleadosEmail")]
        public async Task<ActionResult> getEmpleadosEmail()
        {
            try
            {
                List<EmpleadosEmail> empleados = new List<EmpleadosEmail>();

                using (SqlConnection connection = (SqlConnection)_dbpContext.Database.GetDbConnection())
                {
                    connection.Open();
                    // Nombre del procedimiento almacenado que quieres consultar
                    string storedProcedureName = "SP_CHECADAS_EMPLEADOS_EMAIL";

                    // Crear un comando para ejecutar el procedimiento almacenado
                    SqlCommand command = new SqlCommand(storedProcedureName, connection);
                    command.CommandType = CommandType.StoredProcedure;

                    // Ejecutar el comando y obtener los resultados si es necesario
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            empleados.Add(new EmpleadosEmail() 
                            {
                                idemp = reader.GetInt32(reader.GetOrdinal("ID_EMPLEADO")),
                                idpuesto = reader.GetInt32(reader.GetOrdinal("ID_PUESTO")),
                                jdata = reader.GetString(reader.GetOrdinal("JDATA")),
                                nombre = reader.GetString(reader.GetOrdinal("NOM_TRAB")),
                                ap_paterno = reader.GetString(reader.GetOrdinal("AP_PATERNO")),
                                ap_materno = reader.GetString(reader.GetOrdinal("AP_MATERNO")),
                                nompuesto = reader.GetString(reader.GetOrdinal("NOM_PUESTO"))

                            });
                            // Acceder a los resultados y realizar las operaciones necesarias
                          
                        }
                        reader.Close();
                    }
                    
                }
                return StatusCode(StatusCodes.Status200OK, empleados);
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
        [Route("horascalendario")]
        public async Task<ActionResult> getHorasCalendario()
        {
            try
            {
                List<EmpleadosEmail> empleados = new List<EmpleadosEmail>();

                using (SqlConnection connection = (SqlConnection)_dbpContext.Database.GetDbConnection())
                {
                    connection.Open();
                    // Nombre del procedimiento almacenado que quieres consultar
                    string storedProcedureName = "SP_CHECADAS_EMPLEADOS_EMAIL";

                    // Crear un comando para ejecutar el procedimiento almacenado
                    SqlCommand command = new SqlCommand(storedProcedureName, connection);
                    command.CommandType = CommandType.StoredProcedure;

                    // Ejecutar el comando y obtener los resultados si es necesario
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            empleados.Add(new EmpleadosEmail()
                            {
                                idemp = reader.GetInt32(reader.GetOrdinal("ID_EMPLEADO")),
                                idpuesto = reader.GetInt32(reader.GetOrdinal("ID_PUESTO")),
                                jdata = reader.GetString(reader.GetOrdinal("JDATA")),
                                nombre = reader.GetString(reader.GetOrdinal("NOM_TRAB")),
                                ap_paterno = reader.GetString(reader.GetOrdinal("AP_PATERNO")),
                                ap_materno = reader.GetString(reader.GetOrdinal("AP_MATERNO")),
                                nompuesto = reader.GetString(reader.GetOrdinal("NOM_PUESTO"))

                            });
                            // Acceder a los resultados y realizar las operaciones necesarias

                        }
                        reader.Close();
                    }

                }

                foreach (var empleado in empleados) 
                {
                    dynamic jsonObject = JsonConvert.DeserializeObject<dynamic>(empleado.jdata);
                    if (jsonObject.tipo == 1)
                    {
                        var data = jsonObject.data;
                        Console.WriteLine(data.ToString());
                    }
                    else 
                    {
                        var data = jsonObject.data;
                        Console.WriteLine(data.ToString());
                    }
                }

                return StatusCode(StatusCodes.Status200OK, empleados);
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

    public class checadasModel 
    {
       public DateTime fecha { get; set; }
       public int cla_trab { get; set; }
        public int cla_reloj { get; set; }
        public string nom_reloj { get; set; }
        public string nombre { get; set; }
        public string ap_paterno { get; set; }
        public string ap_materno { get; set; }

    }

    public class CatModel 
    {
        public int id {get; set;}
        public string name { get; set;}
    }

    public class EmpleadoModel
    {
        public int id { get; set;}
        public string nombre { get; set;}
        public string apellidop { get; set;}
        public string apellidom { get; set;}
    }

    public class ExcelModel 
    {
        public string jdata { get; set;}  
        public DateTime fi { get; set;}
        public DateTime ff { get; set;}
    }

    public class EmpleadosEmail 
    {
        public int idpuesto { get; set;}
        public int idemp { get; set; }
        public string nombre { get; set;}
        public string ap_paterno { get;set;}
        public string ap_materno { get; set; }
        public string nompuesto { get;set;}
        public string jdata { get; set;}    
    }
}
