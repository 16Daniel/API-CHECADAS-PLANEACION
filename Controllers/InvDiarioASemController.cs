using API_PEDIDOS.ModelsDB2;
using API_PEDIDOS.ModelsDBP;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace API_PEDIDOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvDiarioASemController : ControllerBase
    {
        private readonly string _connectionString;
        protected DBPContext _dbpContext;
        protected BD2Context _contextdb2;

        public InvDiarioASemController(IConfiguration configuration, DBPContext dbpc, BD2Context db2c)
        {
            _dbpContext = dbpc;
            _contextdb2 = db2c;
            _connectionString = _dbpContext.Database.GetConnectionString();
        }

        [HttpGet]
        public async Task<IActionResult> GetParametros()
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var query = "SELECT ConfiguracionJson FROM ParametrosConfiguracionDiarioASem WHERE NombreClave = 'CONFIG_GENERAL'";
            using var cmd = new SqlCommand(query, conn);

            var jsonResult = await cmd.ExecuteScalarAsync() as string;

            if (string.IsNullOrEmpty(jsonResult))
            {
                return Ok(new ParametrosConfigDto());
            }

            var result = JsonSerializer.Deserialize<ParametrosConfigDto>(jsonResult, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return Ok(result);
        }

        [HttpGet]
        [Route("getArticulosInvDiario")]
        public async Task<IActionResult> GetArticulosInvDiaro()
        {
            try
            {

                var query = from art in _contextdb2.Articulos
                            join artcl in _contextdb2.Articuloscamposlibres on art.Codarticulo equals artcl.Codarticulo
                            into gj
                            from subartcl in gj.DefaultIfEmpty()
                            where subartcl != null && subartcl.RegularizaSemanal == "T" && art.Descatalogado == "F"
                            select new
                            {
                                cod = art.Codarticulo,
                                descripcion = art.Descripcion,
                                marca = art.Marca,
                            };
                var data = query.ToList();
                return StatusCode(200, data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = ex.ToString(),
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GuardarParametros([FromBody] ParametrosConfigDto dto)
        {
            string jsonString = JsonSerializer.Serialize(dto);

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var query = @"
                MERGE ParametrosConfiguracionDiarioASem AS target
                USING (SELECT 'CONFIG_GENERAL' AS NombreClave) AS source
                ON (target.NombreClave = source.NombreClave)
                WHEN MATCHED THEN
                    UPDATE SET ConfiguracionJson = @Json, FechaModificacion = GETDATE()
                WHEN NOT MATCHED THEN
                    INSERT (NombreClave, ConfiguracionJson) VALUES ('CONFIG_GENERAL', @Json);";

            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Json", jsonString);

            await cmd.ExecuteNonQueryAsync();

            return Ok(new { mensaje = "Parámetros guardados correctamente" });
        }
    }

    public class ParametrosConfigDto
    {
        public List<int> SucursalIds { get; set; } = new();
        public List<int> ArticuloIds { get; set; } = new();
    }

    public class CatItemDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }
}
