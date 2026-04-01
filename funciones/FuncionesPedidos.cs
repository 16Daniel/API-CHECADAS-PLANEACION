using API_PEDIDOS.Controllers;
using API_PEDIDOS.ModelsDB2;
using API_PEDIDOS.ModelsDBP;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace API_PEDIDOS.funciones
{
    public class FuncionesPedidos
    {
        private readonly ILogger<FuncionesPedidos> _logger;
        protected BD2Context _contextdb2;
        protected DBPContext _dbpContext;

        public FuncionesPedidos(ILogger<FuncionesPedidos> logger, BD2Context db2c, DBPContext dbpc) 
        {
            _logger = logger;
            _contextdb2 = db2c;
            _dbpContext = dbpc;
        }

        public void eliminarPedidosDuplicados() 
        {
            // Cadena de conexión a tu base de datos SQL Server
            string connectionString = _dbpContext.Database.GetConnectionString();

            // Crear la conexión
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    // Abrir la conexión
                    connection.Open();

                    // Crear el comando para ejecutar el procedimiento almacenado
                    using (SqlCommand command = new SqlCommand("ELIMINAR_PEDIDOS_DUPLICADOS", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        // Ejecutar el procedimiento almacenado
                        int rowsAffected = command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al ejecutar el procedimiento: " + ex.Message);
                }
                finally
                {
                    // Cerrar la conexión si está abierta
                    if (connection.State == System.Data.ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
        }

        public async Task<double> obtenerInvnetarioArtSemanal(int codsuc, int codart) 
        {
            string _connectionString = _dbpContext.Database.GetConnectionString();
            string codalm = "";
            if (codsuc < 10)
            {
                codalm = "0" + codsuc;
            }
            else { codalm = codsuc.ToString(); }
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@FECHA", DateTime.Now.ToString("dd/MM/yyyy"), DbType.String, ParameterDirection.Input);
                parameters.Add("@CODALM", codalm, DbType.String, ParameterDirection.Input);
                parameters.Add("@CODART", codart, DbType.Int32, ParameterDirection.Input);

                // Ejecutar el SP y obtener el resultado
                var resultado = connection.QueryFirstOrDefault<double>(
                    "SP_GET_INV_ART_SEMANAL",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return resultado;
            }
        }

        public async Task<List<PinventarioModel>> getInventario(int codsuc,int codart) 
        {
            List<PinventarioModel> inventarios = new List<PinventarioModel>();

            string _connectionString = _dbpContext.Database.GetConnectionString();

            using (var conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("SP_GET_INVENTARIO", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    string codalm = "";
                    if (codsuc < 10)
                    {
                        codalm = "0" + codsuc;
                    }
                    else { codalm = codsuc.ToString(); }
                    // Añadir parámetros al comando
                    command.Parameters.Add("@sucursal", SqlDbType.NVarChar, 5).Value = codalm;
                    command.Parameters.Add("@articulo", SqlDbType.Int).Value = codart;
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
            return inventarios; 
        }

        public async Task<List<PinventarioModel>> getInventarioTeorico(int codsuc, int codart)
        {
            List<PinventarioModel> inventarios = new List<PinventarioModel>();

            string _connectionString = _dbpContext.Database.GetConnectionString();

            using (var conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("SPS_GET_DIFERENCIA_LIN", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    string codalm = "";
                    if (codsuc < 10)
                    {
                        codalm = "0" + codsuc;
                    }
                    else { codalm = codsuc.ToString(); }
                    // Añadir parámetros al comando
                    command.Parameters.Add("@FECHA", System.Data.SqlDbType.VarChar, 10).Value = DateTime.Now.ToString("dd/MM/yyyy");
                    command.Parameters.Add("@CODALM", System.Data.SqlDbType.NVarChar, 10).Value = codalm;
                    command.Parameters.Add("@CODART", System.Data.SqlDbType.Int).Value = codart;
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
            return inventarios;
        }

    }
}
