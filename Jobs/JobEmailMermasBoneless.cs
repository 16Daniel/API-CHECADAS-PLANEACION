using API_PEDIDOS.ModelsDB2;
using API_PEDIDOS.ModelsDBP;
using Microsoft.Data.SqlClient;
using Quartz;
using System.Data;
using System.Net.Mail;
using System.Net;
using System.Text;

namespace API_PEDIDOS.Jobs
{
    public class QuartzHostedServiceMermasBoneless : IHostedService
    {
        private readonly IScheduler _scheduler;

        public QuartzHostedServiceMermasBoneless(IScheduler scheduler)
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
    public class JobEmailMermasBoneless : IJob
    {
        protected BD2Context _contextdb2;
        protected DBPContext _dbpContext;
        private readonly IConfiguration _configuration;

        private readonly ILogger<JobEmail> _logger;
        public string connectionString = string.Empty;


        public JobEmailMermasBoneless(ILogger<JobEmail> logger, BD2Context db2c, DBPContext dbpc, IConfiguration configuration)
        {
            _logger = logger;
            _dbpContext = dbpc;
            _contextdb2 = db2c;
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DB2");
        }

        public async Task Execute(IJobExecutionContext context)
        {

            // Define el nombre del procedimiento almacenado
            string storedProcedureName = "JOBEMAIL_MERMAS_BONELESS";

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
                        command.CommandTimeout = 1000;

                        // Ejecutar el comando y obtener un SqlDataReader
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Procesar los resultados
                            while (reader.Read())
                            {

                                // Obtiene la fecha actual
                                DateTime fechaActual = DateTime.Now;

                                // Obtiene el día actual
                                int diaActual = fechaActual.Day;

                                string nombreMes = "";
                                // Valida si el día actual es 1
                                if (diaActual == 1)
                                {
                                    // Obtiene el mes anterior
                                    DateTime mesAnterior = fechaActual.AddMonths(-1);

                                    // Formatea el nombre del mes anterior
                                    nombreMes = mesAnterior.ToString("MMMM");
                                }
                                else
                                {
                                    nombreMes = fechaActual.ToString("MMMM");
                                }
                                nombreMes = nombreMes.ToUpper();
                                // Asumiendo que la columna BODYEMAIL es la primera columna en el resultado
                                string bodyEmail = reader["BODYEMAIL"].ToString();

                                bodyEmail = bodyEmail.Replace("--mes", nombreMes);
                                EnviarCorreo(bodyEmail, nombreMes);
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

        static void EnviarCorreo(string bodymail, string nombremes)
        {
            //// Configurar la información de la cuenta de Gmail
            string correoRemitente = "gilberto.r@operamx.com";
            string contraseña = "yrhb lxno riph bdtc";

            //// Configurar la información de la cuenta de Gmail
            //string correoRemitente = "it_token@operamx.com";
            //string contraseña = "M@5TERKEY";

            // Configurar la información del destinatario
            string correoDestinatario = "enrique.j@operamx.com";
            //string correoDestinatario = "arturo.m@operamx.com";
            string asunto = "❌ IT: MERMAS BONELESS " + nombremes;

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
            mensaje.To.Add("gilberto.r@operamx.com");
            mensaje.Bcc.Add("arturo.m@operamx.com");
            mensaje.To.Add("carlos.c@operamx.com");
            mensaje.To.Add("adrian.c@operamx.com");
            mensaje.To.Add("jorge.j@operamx.com");
            mensaje.Bcc.Add("daniel.h@operamx.com");


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
