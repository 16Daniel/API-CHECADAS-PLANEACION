using API_PEDIDOS.ModelsDB2;
using API_PEDIDOS.ModelsDBP;
using Quartz;
using System.Net.Mail;
using System.Net;
using System.Text;
using Microsoft.Data.SqlClient;
using System.Data;

namespace API_PEDIDOS.Jobs
{
    public class QuartzHostedService25pts : IHostedService
    {
        private readonly IScheduler _scheduler;

        public QuartzHostedService25pts(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _scheduler.Start(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _scheduler.Shutdown(cancellationToken);
        }
    }

    [DisallowConcurrentExecution]
    public class JobEmail25pts : IJob
    {
        protected BD2Context _contextdb2;
        protected DBPContext _dbpContext;
        private readonly IConfiguration _configuration;

        private readonly ILogger<JobEmail> _logger;
        public string connectionString = string.Empty;


        public JobEmail25pts(ILogger<JobEmail> logger, BD2Context db2c, DBPContext dbpc, IConfiguration configuration)
        {
            _logger = logger;
            _dbpContext = dbpc;
            _contextdb2 = db2c;
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DBREBELWINGS");
        }

        public async Task Execute(IJobExecutionContext context)
        {

            // Define el nombre del procedimiento almacenado
            string storedProcedureName = "EMAIL_25PTS";

            // Crear la conexión a la base de datos
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    // Abrir la conexión
                    connection.Open();

                    // Crear un comando para ejecutar el procedimiento almacenado
                    using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Ejecutar el comando y obtener un SqlDataReader
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Procesar los resultados
                            while (reader.Read())
                            {
                                // Asumiendo que la columna BODYEMAIL es la primera columna en el resultado
                                string bodyEmail = reader["BODYEMAIL"].ToString();
                                EnviarCorreo(bodyEmail);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Manejar cualquier error que ocurra
                    Console.WriteLine("Ocurrió un error: " + ex.Message);
                }
            }
        }

        static void EnviarCorreo(string bodymail)
        {
            //// Configurar la información de la cuenta de Gmail
            string correoRemitente = "gilberto.r@operamx.com";
            string contraseña = "GRC1931519315";

            //// Configurar la información de la cuenta de Gmail
            //string correoRemitente = "it_token@operamx.com";
            //string contraseña = "M@5TERKEY";

            // Configurar la información del destinatario
            // string correoDestinatario = "developeramh@outlook.com";
            string correoDestinatario = "arturo.m@operamx.com";
            string asunto = "🚦IT: REPORTE 25 PTS";

            // Configurar el cliente SMTP de Gmail
            SmtpClient clienteSmtp = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(correoRemitente, contraseña),
                EnableSsl = true,
            };

            // Crear el mensaje de correo
            MailMessage mensaje = new MailMessage(correoRemitente, correoDestinatario, asunto, string.Empty)
            {
                IsBodyHtml = true,
                Body = bodymail,
                SubjectEncoding = Encoding.UTF8,
                BodyEncoding = Encoding.UTF8
            };

            mensaje.Bcc.Add("daniel.h@operamx.com");
            mensaje.Bcc.Add("gilberto.r@operamx.com");
            mensaje.To.Add("enrique.j@operamx.com");
            mensaje.To.Add("carlos.c@operamx.com");
            mensaje.To.Add("adrian.c@operamx.com");
            mensaje.To.Add("jorge.j@operamx.com");
            mensaje.To.Add("ricardo.s@operamx.com");


            try
            {
                // Enviar el mensaje
                clienteSmtp.Send(mensaje);
                Console.WriteLine("Correo enviado con éxito.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al enviar el correo: {ex.Message}");
            }
            finally
            {
                // Liberar recursos
                mensaje.Dispose();
            }
        }
    }
}
