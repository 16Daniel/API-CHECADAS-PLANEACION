using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class TelecomandaVersion
    {
        public int? Id { get; set; }
        public string? VersionSucursal { get; set; }
        public string? VersionServidor { get; set; }
        public string? LocalIp { get; set; }
        public string DbName { get; set; } = null!;
        public string Pc { get; set; } = null!;
        public DateTime? FechaActualizacion { get; set; }
        public TimeSpan? HoraActualizacion { get; set; }
        public string? Codalmacen { get; set; }
        public string? Nombrealmacen { get; set; }
    }
}
