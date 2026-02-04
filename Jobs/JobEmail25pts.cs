using API_PEDIDOS.ModelsDB2;
using API_PEDIDOS.ModelsDBP;
using Quartz;
using System.Net.Mail;
using System.Net;
using System.Text;
using Microsoft.Data.SqlClient;
using System.Data;
using API_PEDIDOS.Controllers;
using Newtonsoft.Json;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Drawing;
using System.Xml.Linq;

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
            string storedProcedureName = "DETALLES_EMAIL_25PTS";
            string storedProcedureName2 = "EMAIL_25PTS";
            DataSet _dataSet = new DataSet();

            // Crear la conexión a la base de datos
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    // Abrir la conexión
                    connection.Open();


                    using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            // Llenar el DataSet con los resultados del procedimiento almacenado
                            adapter.Fill(_dataSet);
                            
                        }
                    }

                }
                catch (Exception ex)
                {
                    // Manejar cualquier error que ocurra
                    Console.WriteLine("Ocurrió un error: " + ex.Message);
                }
            }


            // Crear la conexión a la base de datos
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    // Abrir la conexión
                    connection.Open();

                    // Crear un comando para ejecutar el procedimiento almacenado
                    using (SqlCommand command = new SqlCommand(storedProcedureName2, connection))
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
                                //EnviarCorreo(bodyEmail);
                                detalles25pts(_dataSet,bodyEmail);

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

        static void EnviarCorreo(MemoryStream file, string bodymail, string mes)
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
            string asunto = "🚦IT: REPORTE 25 PTS "+mes;

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
            mensaje.To.Add("ricardo.s@operamx.com");

            // Create the attachment from the MemoryStream
            file.Position = 0; // Ensure the stream position is at the beginning
            var attachment = new Attachment(file, "DETALLES 25PTS.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            mensaje.Attachments.Add(attachment);

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

        public void detalles25pts(DataSet data,string emailbody)
        {
           

            Color colorcelda = ColorTranslator.FromHtml("#00000000");
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            // Crear un nuevo archivo de Excel
            using (var package = new ExcelPackage())
            {
                // Agregar una hoja al libro de trabajo
                var worksheet = package.Workbook.Worksheets.Add("30 DIAS");

                worksheet.Cells[1, 1].Value = "FECHA";
                worksheet.Cells[1, 2].Value = "SUCURSAL";
                worksheet.Cells[1, 3].Value = "USUARIO";
                worksheet.Cells[1, 4].Value = "VENDEDOR";
                worksheet.Cells[1, 5].Value = "SALA";
                worksheet.Cells[1, 6].Value = "MESA";
                worksheet.Cells[1, 7].Value = "TOTAL AYC";
                worksheet.Cells[1, 8].Value = "COBROS";
                worksheet.Cells[1, 9].Value = "COBROS MÍNIMO";
                worksheet.Cells[1, 10].Value = "DIFERENCIA";
                worksheet.Cells[1, 11].Value = "JUSTIFICACIÓN";
                


                using (var range = worksheet.Cells["A1:K1"])
                {
                    Color colorFondo = ColorTranslator.FromHtml("#00000000");
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(colorFondo);
                    range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    range.AutoFitColumns();
                }
                int contador = 2;


                DataTable tblayer = data.Tables[0]; 

                for (int i = 0; i < tblayer.Rows.Count; i++)
                {
                    worksheet.Cells[contador, 1].Value = tblayer.Rows[i][1];
                    worksheet.Cells[contador, 1].Style.Numberformat.Format = "yyyy-mm-dd";
                    worksheet.Cells[contador, 2].Value = tblayer.Rows[i][10];
                    worksheet.Cells[contador, 3].Value = tblayer.Rows[i][9];
                    worksheet.Cells[contador, 4].Value = tblayer.Rows[i][11];
                    worksheet.Cells[contador, 5].Value = tblayer.Rows[i][2];
                    worksheet.Cells[contador, 6].Value = tblayer.Rows[i][3];
                    worksheet.Cells[contador, 7].Value = tblayer.Rows[i][4];
                    worksheet.Cells[contador, 8].Value = tblayer.Rows[i][5];
                    worksheet.Cells[contador, 9].Value = tblayer.Rows[i][6];
                    worksheet.Cells[contador, 10].Value = tblayer.Rows[i][7];
                    worksheet.Cells[contador, 11].Value = tblayer.Rows[i][8];

                    contador++;

                }


                using (var range = worksheet.Cells)
                {
                    range.AutoFitColumns();
                }


                //////////////////////////
                /////
                //  // Agregar una hoja al libro de trabajo
                //var worksheet7 = package.Workbook.Worksheets.Add("7 DIAS");

                //worksheet7.Cells[1, 1].Value = "FECHA";
                //worksheet7.Cells[1, 2].Value = "SUCURSAL";
                //worksheet7.Cells[1, 3].Value = "USUARIO";
                //worksheet7.Cells[1, 4].Value = "VENDEDOR";
                //worksheet7.Cells[1, 5].Value = "SALA";
                //worksheet7.Cells[1, 6].Value = "MESA";
                //worksheet7.Cells[1, 7].Value = "TOTAL AYC";
                //worksheet7.Cells[1, 8].Value = "COBROS";
                //worksheet7.Cells[1, 9].Value = "COBROS MÍNIMO";
                //worksheet7.Cells[1, 10].Value = "DIFERENCIA";
                //worksheet7.Cells[1, 11].Value = "JUSTIFICACIÓN";



                //using (var range = worksheet7.Cells["A1:K1"])
                //{
                //    Color colorFondo = ColorTranslator.FromHtml("#00000000");
                //    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                //    range.Style.Fill.BackgroundColor.SetColor(colorFondo);
                //    range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                //    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                //    range.AutoFitColumns();
                //}
                //contador = 2;


                //DataTable tbl7d = data.Tables[1];

                //for (int i = 0; i < tbl7d.Rows.Count; i++)
                //{
                //    worksheet7.Cells[contador, 1].Value = tbl7d.Rows[i][1];
                //    worksheet7.Cells[contador, 1].Style.Numberformat.Format = "yyyy-mm-dd";
                //    worksheet7.Cells[contador, 2].Value = tbl7d.Rows[i][10];
                //    worksheet7.Cells[contador, 3].Value = tbl7d.Rows[i][9];
                //    worksheet7.Cells[contador, 4].Value = tbl7d.Rows[i][11];
                //    worksheet7.Cells[contador, 5].Value = tbl7d.Rows[i][2];
                //    worksheet7.Cells[contador, 6].Value = tbl7d.Rows[i][3];
                //    worksheet7.Cells[contador, 7].Value = tbl7d.Rows[i][4];
                //    worksheet7.Cells[contador, 8].Value = tbl7d.Rows[i][5];
                //    worksheet7.Cells[contador, 9].Value = tbl7d.Rows[i][6];
                //    worksheet7.Cells[contador, 10].Value = tbl7d.Rows[i][7];
                //    worksheet7.Cells[contador, 11].Value = tbl7d.Rows[i][8];
                //    contador++;

                //}


                //using (var range = worksheet7.Cells)
                //{
                //    range.AutoFitColumns();
                //}

                //////////////////////////////////////////////////////////////////////////////////


                //// Agregar una hoja al libro de trabajo
                //var worksheet14 = package.Workbook.Worksheets.Add("14 DIAS");

                //worksheet14.Cells[1, 1].Value = "FECHA";
                //worksheet14.Cells[1, 2].Value = "SUCURSAL";
                //worksheet14.Cells[1, 3].Value = "USUARIO";
                //worksheet14.Cells[1, 4].Value = "VENDEDOR";
                //worksheet14.Cells[1, 5].Value = "SALA";
                //worksheet14.Cells[1, 6].Value = "MESA";
                //worksheet14.Cells[1, 7].Value = "TOTAL AYC";
                //worksheet14.Cells[1, 8].Value = "COBROS";
                //worksheet14.Cells[1, 9].Value = "COBROS MÍNIMO";
                //worksheet14.Cells[1, 10].Value = "DIFERENCIA";
                //worksheet14.Cells[1, 11].Value = "JUSTIFICACIÓN";



                //using (var range = worksheet14.Cells["A1:K1"])
                //{
                //    Color colorFondo = ColorTranslator.FromHtml("#00000000");
                //    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                //    range.Style.Fill.BackgroundColor.SetColor(colorFondo);
                //    range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                //    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                //    range.AutoFitColumns();
                //}
                //contador = 2;


                //DataTable tbl14 = data.Tables[2];

                //for (int i = 0; i < tbl14.Rows.Count; i++)
                //{
                //    worksheet14.Cells[contador, 1].Value = tbl14.Rows[i][1];
                //    worksheet14.Cells[contador, 1].Style.Numberformat.Format = "yyyy-mm-dd";
                //    worksheet14.Cells[contador, 2].Value = tbl14.Rows[i][10];
                //    worksheet14.Cells[contador, 3].Value = tbl14.Rows[i][9];
                //    worksheet14.Cells[contador, 4].Value = tbl14.Rows[i][11];
                //    worksheet14.Cells[contador, 5].Value = tbl14.Rows[i][2];
                //    worksheet14.Cells[contador, 6].Value = tbl14.Rows[i][3];
                //    worksheet14.Cells[contador, 7].Value = tbl14.Rows[i][4];
                //    worksheet14.Cells[contador, 8].Value = tbl14.Rows[i][5];
                //    worksheet14.Cells[contador, 9].Value = tbl14.Rows[i][6];
                //    worksheet14.Cells[contador, 10].Value = tbl14.Rows[i][7];
                //    worksheet14.Cells[contador, 11].Value = tbl14.Rows[i][8];
                //    contador++;

                //}


                //using (var range = worksheet14.Cells)
                //{
                //    range.AutoFitColumns();
                //}

                /////////////////


                //// Agregar una hoja al libro de trabajo
                //var worksheetm = package.Workbook.Worksheets.Add("MENSUAL");

                //worksheetm.Cells[1, 1].Value = "FECHA";
                //worksheetm.Cells[1, 2].Value = "SUCURSAL";
                //worksheetm.Cells[1, 3].Value = "USUARIO";
                //worksheetm.Cells[1, 4].Value = "VENDEDOR";
                //worksheetm.Cells[1, 5].Value = "SALA";
                //worksheetm.Cells[1, 6].Value = "MESA";
                //worksheetm.Cells[1, 7].Value = "TOTAL AYC";
                //worksheetm.Cells[1, 8].Value = "COBROS";
                //worksheetm.Cells[1, 9].Value = "COBROS MÍNIMO";
                //worksheetm.Cells[1, 10].Value = "DIFERENCIA";
                //worksheetm.Cells[1, 11].Value = "JUSTIFICACIÓN";



                //using (var range = worksheetm.Cells["A1:K1"])
                //{
                //    Color colorFondo = ColorTranslator.FromHtml("#00000000");
                //    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                //    range.Style.Fill.BackgroundColor.SetColor(colorFondo);
                //    range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                //    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                //    range.AutoFitColumns();
                //}
                //contador = 2;


                //DataTable tblmes = data.Tables[3];

                //for (int i = 0; i < tblmes.Rows.Count; i++)
                //{
                //    worksheetm.Cells[contador, 1].Value = tblmes.Rows[i][1];
                //    worksheetm.Cells[contador, 1].Style.Numberformat.Format = "yyyy-mm-dd";
                //    worksheetm.Cells[contador, 2].Value = tblmes.Rows[i][10];
                //    worksheetm.Cells[contador, 3].Value = tblmes.Rows[i][9];
                //    worksheetm.Cells[contador, 4].Value = tblmes.Rows[i][11];
                //    worksheetm.Cells[contador, 5].Value = tblmes.Rows[i][2];
                //    worksheetm.Cells[contador, 6].Value = tblmes.Rows[i][3];
                //    worksheetm.Cells[contador, 7].Value = tblmes.Rows[i][4];
                //    worksheetm.Cells[contador, 8].Value = tblmes.Rows[i][5];
                //    worksheetm.Cells[contador, 9].Value = tblmes.Rows[i][6];
                //    worksheetm.Cells[contador, 10].Value = tblmes.Rows[i][7];
                //    worksheetm.Cells[contador, 11].Value = tblmes.Rows[i][8];
                //    contador++;

                //}


                //using (var range = worksheetm.Cells)
                //{
                //    range.AutoFitColumns();
                //}




                // Configurar la respuesta HTTP para devolver el archivo de Excel
                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

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

                emailbody = emailbody.Replace("--mes", nombreMes); 
                EnviarCorreo(stream, emailbody, nombreMes); 
            }
        }
    }
}
