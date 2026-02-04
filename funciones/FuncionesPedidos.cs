using API_PEDIDOS.ModelsDB2;
using API_PEDIDOS.ModelsDBP;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

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

    }
}
