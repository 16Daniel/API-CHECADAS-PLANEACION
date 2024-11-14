using API_PEDIDOS.Controllers;
using API_PEDIDOS.ModelsDB2;
using API_PEDIDOS.ModelsDBP;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Quartz;
using System.Data;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Globalization;
using System.Text.Json;
using System;
using Newtonsoft.Json;
using System.Dynamic;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;

namespace API_PEDIDOS.Jobs
{

    public class QuartzHostedServiceM : IHostedService
    {
        private readonly IScheduler _scheduler;

        public QuartzHostedServiceM(IScheduler scheduler)
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
    public class JobEmailMes : IJob
    {
        protected BD2Context _contextdb2;
        protected DBPContext _dbpContext;
        private readonly IConfiguration _configuration;

        private readonly ILogger<JobEmailMes> _logger;
        public string connectionString = string.Empty;


        public JobEmailMes(ILogger<JobEmailMes> logger, BD2Context db2c, DBPContext dbpc, IConfiguration configuration)
        {
            _logger = logger;
            _dbpContext = dbpc;
            _contextdb2 = db2c;
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Execute(IJobExecutionContext context)
        {


            try 
            {
                // Obtener el primer día del mes actual
                DateTime primerDiaMesActual = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

                // Restar un mes al primer día del mes actual para obtener el primer día del mes pasado
                DateTime primerDiaMesPasado = primerDiaMesActual.AddMonths(-1);

                // Obtener el último día del mes pasado
                DateTime ultimoDiaMesPasado = primerDiaMesActual.AddDays(-1);

                // Encontrar el último lunes del mes pasado
                DateTime ultimoLunesMesPasado = ultimoDiaMesPasado;
                while (ultimoLunesMesPasado.DayOfWeek != DayOfWeek.Monday)
                {
                    ultimoLunesMesPasado = ultimoLunesMesPasado.AddDays(-1);
                }

                List<DateTime> semanas = new List<DateTime>();

                DateTime temp = ultimoLunesMesPasado;
                temp = temp.AddDays(6);

                if (temp.Month == primerDiaMesPasado.Month)
                {
                    semanas.Add(temp);
                }

                ultimoLunesMesPasado = ultimoLunesMesPasado.AddDays(-7);

                while (ultimoLunesMesPasado.Month == primerDiaMesPasado.Month)
                {
                    semanas.Add(ultimoLunesMesPasado);
                    ultimoLunesMesPasado = ultimoLunesMesPasado.AddDays(-7);
                }

                temp = ultimoLunesMesPasado;
                temp = temp.AddDays(6);

                if (temp.Month == primerDiaMesPasado.Month)
                {
                    semanas.Add(temp);
                }

                semanas.Sort((fecha1, fecha2) => fecha1.CompareTo(fecha2));

                List<List<reporteGenM>> reportessemana = new List<List<reporteGenM>>();

                // Crear un objeto Calendar con la configuración regional española
                Calendar calendario = new CultureInfo("es-ES").Calendar;
                List<int> arrsemanas = new List<int>();
                int semanaAño = 0;
                foreach (var item in semanas)
                {
                    // Obtener la semana del año
                    semanaAño = calendario.GetWeekOfYear(item, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
                    arrsemanas.Add(semanaAño);
                    reportessemana.Add(GetReportesSemana(item));
                }

                String bodyemail = "";
                bodyemail = generateBody(reportessemana, arrsemanas);
                EnviarCorreo(bodyemail);

                Console.WriteLine("Fecha del último lunes del mes pasado: " + ultimoLunesMesPasado.ToString("yyyy-MM-dd"));
            }
            catch (Exception ex) 
            {
                Console.WriteLine("");      
            }

        }

        public List<reporteGenM> GetReportesSemana(DateTime fechainicio) 
        {
            try
            {
                List<reporteGenM> reportes = new List<reporteGenM>();
                List<EmpleadosEmail> empleados = new List<EmpleadosEmail>();

                using (SqlConnection connection = new SqlConnection(connectionString))
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


                DateTime fechaHoy = fechainicio;
                DateTime fecha2 = fechainicio;
                DayOfWeek diaSemana = fecha2.DayOfWeek;
                int numdia = 0;


                // Crear un objeto Calendar con la configuración regional española
                Calendar calendario = new CultureInfo("es-ES").Calendar;

                // Obtener la semana del año
                int semanaAño = calendario.GetWeekOfYear(fechaHoy, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);


                foreach (var item in empleados)
                {
                    fechaHoy = fechainicio;
                    fecha2 = fechainicio;

                    double horasprogramadas = 0;
                    int visitasprogramadas = 0;
                    double totalhoras = 0;
                    int totalvisitas = 0;
                    double cumplimientoH = 0;
                    double cumplimientioV = 0;
                    int semanacalendario = 0;
                    int tipo = 1;
                    for (int i = 0; i < semanaAño; i++)
                    {
                        if (semanacalendario == 4)
                        {
                            semanacalendario = 1;
                        }
                        else { semanacalendario++; }
                    }

                    CalendariosChecada calendarioU = _dbpContext.CalendariosChecadas.Where(c => c.IdPuesto == item.idpuesto && c.IdEmpleado == item.idemp).FirstOrDefault();

                    for (int f = 0; f < 7; f++)
                    {
                        if (f == 0)
                        {
                            fechaHoy = fechaHoy.AddDays(0);
                        }
                        else
                        {
                            fechaHoy = fechaHoy.AddDays(1);
                        }

                        fecha2 = fechaHoy.AddDays(1);
                        diaSemana = fechaHoy.DayOfWeek;
                        numdia = 0;
                        switch (diaSemana)
                        {
                            case DayOfWeek.Monday:
                                numdia = 0;
                                break;
                            case DayOfWeek.Tuesday:
                                numdia = 1;
                                break;
                            case DayOfWeek.Wednesday:
                                numdia = 2;
                                break;
                            case DayOfWeek.Thursday:
                                numdia = 3;
                                break;
                            case DayOfWeek.Friday:
                                numdia = 4;
                                break;
                            case DayOfWeek.Saturday:
                                numdia = 5;
                                break;
                            case DayOfWeek.Sunday:
                                numdia = 6;
                                break;
                            default:
                                break;
                        }

                        List<checadasModel> checadas = getChecadas(fechaHoy, fecha2, item.idemp);
                        dynamic obj = JsonConvert.DeserializeObject<dynamic>(calendarioU.Jdata);

                        if (obj.tipo == 1)
                        {
                            tipo = 1;
                            IEnumerable<dynamic> visitas = obj.data;
                            foreach (var visita in visitas.Where(v => v.numsemana == semanacalendario && v.numdia == numdia))
                            {
                                string he = visita.horaEntrada;
                                string[] h1 = he.Split(':');

                                string hs = visita.HoraSalida;
                                string[] h2 = hs.Split(':');

                                // Definir las horas

                                TimeSpan horaInicio = new TimeSpan(int.Parse(h1[0]), int.Parse(h1[1]), 0); // 10:00
                                TimeSpan horaFin = new TimeSpan(int.Parse(h2[0]), int.Parse(h2[1]), 0);   // 15:00

                                // Calcular la diferencia de horas
                                TimeSpan diferencia = horaFin - horaInicio;

                                horasprogramadas = horasprogramadas + diferencia.TotalHours;
                            }
                            visitasprogramadas = visitasprogramadas + visitas.Where(v => v.numsemana == semanacalendario && v.numdia == numdia).Count();
                        }
                        else
                        {
                            tipo = 2;
                            switch (diaSemana)
                            {
                                case DayOfWeek.Monday:
                                    horasprogramadas = horasprogramadas + double.Parse(string.Concat("", obj.data[0]));
                                    break;
                                case DayOfWeek.Tuesday:
                                    horasprogramadas = horasprogramadas + double.Parse(string.Concat("", obj.data[1]));
                                    break;
                                case DayOfWeek.Wednesday:
                                    horasprogramadas = horasprogramadas + double.Parse(string.Concat("", obj.data[2]));
                                    break;
                                case DayOfWeek.Thursday:
                                    horasprogramadas = horasprogramadas + double.Parse(string.Concat("", obj.data[3]));
                                    break;
                                case DayOfWeek.Friday:
                                    horasprogramadas = horasprogramadas + double.Parse(string.Concat("", obj.data[4]));
                                    break;
                                case DayOfWeek.Saturday:
                                    horasprogramadas = horasprogramadas + double.Parse(string.Concat("", obj.data[5]));
                                    break;
                                case DayOfWeek.Sunday:
                                    horasprogramadas = horasprogramadas + double.Parse(string.Concat("", obj.data[6]));
                                    break;
                                default:
                                    break;
                            }
                        }

                        var checadas2 = checadas.Where(c => c.fecha.ToString("yyyy-MM-dd") == fecha2.ToString("yyyy-MM-dd")).ToList();
                        var checadashoy = checadas.Where(c => c.fecha.ToString("yyyy-MM-dd") == fechaHoy.ToString("yyyy-MM-dd")).ToList();
                        var ubicacionesdistintas = checadashoy.Select(s => s.cla_reloj).Distinct().ToList();
                        totalvisitas = totalvisitas + ubicacionesdistintas.Count();

                        foreach (var ubicacion in ubicacionesdistintas)
                        {
                            var regxsuc = checadashoy.Where(c => c.cla_reloj == ubicacion).ToList();
                            if (regxsuc.Count() % 2 == 0)
                            {
                                for (int i = 0; i < regxsuc.Count(); i = i + 2)
                                {
                                    totalhoras = totalhoras + getHourDifference(regxsuc.ElementAt(i).fecha, regxsuc.ElementAt(i + 1).fecha);
                                }
                            }
                            else
                            {
                                if (regxsuc.Count() == 1)
                                {
                                    DateTime fs;
                                    var regxsuc2 = checadas2.Where(c => c.cla_reloj == ubicacion).ToList();
                                    if (regxsuc2.Count() > 0)
                                    {
                                        var temp = regxsuc2.Where(objeto => objeto.fecha.Hour < 4);
                                        if (temp.Count() > 0)
                                        {
                                            fs = temp.ElementAt(temp.Count() - 1).fecha;
                                            totalhoras = totalhoras + getHourDifference(regxsuc.ElementAt(0).fecha, fs);
                                        }
                                    }
                                }
                                else
                                {
                                    if (regxsuc.Count() == 3)
                                    {
                                        totalhoras = totalhoras + getHourDifference(regxsuc.ElementAt(1).fecha, regxsuc.ElementAt(2).fecha);
                                    }
                                }
                            }
                        }

                    }

                    if (visitasprogramadas == 0)
                    {
                        cumplimientioV = 0;
                    }
                    else
                    {
                        if (totalvisitas >= visitasprogramadas)
                        {
                            cumplimientioV = 100;
                        }
                        else
                        {
                            cumplimientioV = (100 / visitasprogramadas) * totalvisitas;
                        }

                    }

                    if (horasprogramadas == 0)
                    {
                        cumplimientoH = 0;
                    }
                    else
                    {
                        if (totalhoras > horasprogramadas)
                        {
                            cumplimientoH = 100;
                        }
                        else
                        {
                            cumplimientoH = (100 / horasprogramadas) * totalhoras;
                        }
                    }
                    if (totalhoras == 0)
                    {
                        cumplimientoH = 0;
                    }


                    reportes.Add(new reporteGenM()
                    {
                        idpuesto = item.idpuesto,
                        puesto = item.nompuesto,
                        //nombre = item.nombre + " " + item.ap_paterno + " " + item.ap_materno,
                        nombre = item.nombre,
                        apellidop = item.ap_paterno,
                        apellidom = item.ap_materno,
                        horasprogramadas = horasprogramadas,
                        visitasprogramadas = visitasprogramadas,
                        totalHoras = totalhoras,
                        totalvisitas = totalvisitas,
                        cumplimientoH = cumplimientoH,
                        cumplimientoV = cumplimientioV,
                        tipo = tipo
                    });
                }

                return reportes;
            }
            catch (Exception ex)
            {
                return new List<reporteGenM>(); 
            }
        }

        public double getHourDifference(DateTime hi, DateTime hf) 
        {
            // Definir las horas

            TimeSpan horaInicio = new TimeSpan(hi.Hour,hi.Minute,hi.Second); 
            TimeSpan horaFin = new TimeSpan(hf.Hour,hf.Minute,hf.Second);

            if (hf.Date > hi.Date)
            {
                TimeSpan diferencia = horaFin - horaInicio;
                return 24+diferencia.TotalHours;
            }
            else 
            {
                // Calcular la diferencia de horas
                TimeSpan diferencia = horaFin - horaInicio;
                return diferencia.TotalHours;
            }

            

           
        }

        public List<checadasModel> getChecadas(DateTime fi, DateTime ff, int iduser) 
        {
            List<checadasModel> checadas = new List<checadasModel>();

            using (SqlConnection connection = new SqlConnection(connectionString))
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
                    try { nomreloj = (string)reader["NOM_RELOJ"]; } catch { }

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

            return checadas;
        }

        public string generateBody(List<List<reporteGenM>> reportes, List<int> arrsemanas) 
        {
            int numsem = reportes.Count;

            string headsemanas = "";

            for (int i = 0; i < arrsemanas.Count; i++) 
            {
                headsemanas += "<th>"+arrsemanas[i]+"</th>";
            }

            string template = @"
<!DOCTYPE html>
<html lang=""es"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Document</title>
    <style>
        .text-center
        {
            text-align: center;
        }

        table {
  border-collapse: collapse;
  border: 1px solid #ccc;
width: 100%;

}
td,th
{ 
    border: 1px solid #bbb;
    font-weight: bold;
    font-size:small;
 text-align: center;
}


@media screen and (min-width: 768px) {
    td,th
{ 
    font-size:medium;
}
    }

    @media screen and (max-width: 400px) {
        td,th
{ 
    font-size:x-small;
}
    }


    </style>
</head>
<body>
    
<h4 class=""text-center"" style=""background-color: #c7d0d3; padding: 10px; border-radius: 10px; width: auto;"">EMPLEADOS CON CALENDARIO</h4>

<table>
<thead>
    <tr style=""background-color: black; color: white;"">
        <th colspan=""2""></th>
        <th colspan=""--colsemanas"">SEMANAS</th>
        <th></th>
    </tr>
    <tr style=""background-color: black; color: white;"">
        <th>PUESTO</th>
        <th>NOMBRE</th>
        --semanas
        <th>TOTAL</th>
    </tr>
</thead>
<tbody>
    --empleadoscalendario
</tbody>
</table>


<h4 class=""text-center"" style=""background-color: #c7d0d3; padding: 10px; border-radius: 10px;"">EMPLEADOS SIN CALENDARIO</h4>

<table>
    <thead>
        <tr style=""background-color: black; color: white;"">
            <th colspan=""2""></th>
            <th colspan=""--colsemanas"">SEMANAS</th>
            <th></th>
        </tr>
        <tr style=""background-color: black; color: white;"">
            <th>PUESTO</th>
            <th>NOMBRE</th>
            --semanas
            <th>TOTAL</th>
        </tr>
    </thead>
    <tbody>
        --empleadoshoras
    </tbody>
    </table>
</body>
</html>
    ";
            string empleadoscalendario = "";
            string empleadoshoras = "";




            var puestosdistintos = reportes[0].Select(r => r.idpuesto).Distinct().ToList();

            List<colorR> colores = new List<colorR>();
            Random random = new Random();
            foreach (var p in puestosdistintos)
            {
                int titulo = 0;
                // Generar valores aleatorios para los componentes RGB
                int r = random.Next(188, 256); // R en el rango [128, 255]
                int g = random.Next(188, 256); // G en el rango [128, 255]
                int b = random.Next(188, 256); // B en el rango [128, 255]

                // Convertir los componentes RGB en un color hexadecimal
                string colorHex = String.Format("#{0:X2}{1:X2}{2:X2}", r, g, b);

                colores.Add(new colorR() { idpuesto = p, color = colorHex });

                var reportesxp = reportes[0].Where(r => r.idpuesto == p).ToList();
            }



            var reportesC = reportes[0].Where(r => r.tipo == 1).ToList();
            var puestosdistintosC = reportesC.Select(r => r.idpuesto).Distinct().ToList();
            var reportesH = reportes[0].Where(r => r.tipo == 2).ToList();
            var puestosdistintosH = reportesH.Select(r => r.idpuesto).Distinct().ToList();


            foreach (var rc in reportesC)
            {
                var color = colores.Where(c => c.idpuesto == rc.idpuesto).FirstOrDefault();
                empleadoscalendario += "<tr>";
                empleadoscalendario += "<td style='background-color:" + color.color + ";'>" + rc.puesto + "</td>";
                empleadoscalendario += "<td style='background-color:" + color.color + ";'>" + rc.nombre + "</td>";

                double acumulado = 0;   
                for(int i=0; i< reportes.Count; i++) 
                {  
                    var emp = reportes[i].Where(r => r.nombre == rc.nombre && r.apellidom == rc.apellidom && r.apellidop == rc.apellidop && r.idpuesto == rc.idpuesto && r.tipo == rc.tipo).FirstOrDefault();
                    if (emp.cumplimientoV < 75)
                    {
                        empleadoscalendario += "<td style='background-color:" + color.color + "; color:red;'>" + emp.cumplimientoV.ToString("N2") + "</td>";
                    }
                    else
                    {
                        empleadoscalendario += "<td style='background-color:" + color.color + ";'>" + emp.cumplimientoV.ToString("N2") + "</td>";
                    }
                    acumulado += emp.cumplimientoV; 
                }
                double promedio = acumulado/reportes.Count();   

                if (promedio < 75)
                {
                    empleadoscalendario += "<td style='background-color:" + color.color + "; color:red;'>" + promedio.ToString("N2") + "</td>";
                }
                else
                {
                    empleadoscalendario += "<td style='background-color:" + color.color + ";'>" + promedio.ToString("N2") + "</td>";
                }

                empleadoscalendario += "</tr>";
            }

            foreach (var rh in reportesH)
            {
                var color = colores.Where(c => c.idpuesto == rh.idpuesto).FirstOrDefault();
                empleadoshoras += "<tr>";
                empleadoshoras += "<td style='background-color:" + color.color + ";'>" + rh.puesto + "</td>";
                empleadoshoras += "<td style='background-color:" + color.color + ";'>" + rh.nombre + "</td>";
                double acumulado = 0;
                for (int i = 0; i < reportes.Count; i++)
                {
                    var emp = reportes[i].Where(r => r.nombre == rh.nombre && r.apellidom == rh.apellidom && r.apellidop == rh.apellidop && r.idpuesto == rh.idpuesto && r.tipo == rh.tipo).FirstOrDefault();

                    if (emp.cumplimientoH < 75)
                    {
                        empleadoshoras += "<td style='background-color:" + color.color + "; color:red;'>" + emp.cumplimientoH.ToString("N2") + "</td>";
                    }
                    else
                    {
                        empleadoshoras += "<td style='background-color:" + color.color + ";'>" + emp.cumplimientoH.ToString("N2") + "</td>";
                    }
                    acumulado += emp.cumplimientoH;
                }
                double promedio = acumulado / reportes.Count();

                if (promedio < 75)
                {
                    empleadoshoras += "<td style='background-color:" + color.color + "; color:red;'>" + promedio.ToString("N2") + "</td>";
                }
                else
                {
                    empleadoshoras += "<td style='background-color:" + color.color + ";'>" + promedio.ToString("N2") + "</td>";
                }

                empleadoshoras += "</tr>";
            }

            template = template.Replace("--semanas", headsemanas);
            template = template.Replace("--colsemanas", numsem.ToString());
            template = template.Replace("--empleadoscalendario", empleadoscalendario);
            template = template.Replace("--empleadoshoras", empleadoshoras);
            return template;
        }

       

        static void EnviarCorreo(string bodymail)
        {

            // Obtener el mes actual
            DateTime fechaActual = DateTime.Today;

            // Obtener el primer día del mes anterior
            DateTime primerDiaMesAnterior = fechaActual.AddMonths(-1).AddDays(-fechaActual.Day + 1);

            // Obtener el nombre del mes anterior en español
            string nombreMesAnterior = primerDiaMesAnterior.ToString("MMMM", CultureInfo.CreateSpecificCulture("es"));


            //// Configurar la información de la cuenta de Gmail
            string correoRemitente = "gilberto.r@operamx.com";
            string contraseña = "GRC1931519315";

            //// Configurar la información de la cuenta de Gmail
            //string correoRemitente = "it_token@operamx.com";
            //string contraseña = "M@5TERKEY";

            // Configurar la información del destinatario
            // string correoDestinatario = "developeramh@outlook.com";
            string correoDestinatario = "arturo.m@operamx.com";
            string asunto = "⚠️ IT: RESUMEN ASISTENCIAS DE " + nombreMesAnterior.ToUpper();

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

            mensaje.To.Add("gilberto.r@operamx.com");
            mensaje.To.Add("enrique.j@operamx.com");
            mensaje.To.Add("adrian.c@operamx.com");
            mensaje.To.Add("jorge.j@operamx.com");
            mensaje.To.Add("ricardo.s@operamx.com");
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


    public class reporteGenM
    {
        public int idpuesto { get; set; }
        public string puesto { get; set; }
        public string nombre { get; set; }
        public string apellidop { get; set; }
        public string apellidom { get; set; }
        public double horasprogramadas { get; set; }
        public int visitasprogramadas { get; set; }
        public double totalHoras { get; set; }
        public int totalvisitas { get; set; }
        public double cumplimientoH { get; set; }
        public double cumplimientoV { get; set; }
        public int tipo { get; set; }
    }

}
