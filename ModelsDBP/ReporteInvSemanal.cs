using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDBP
{
    public partial class ReporteInvSemanal
    {
        public string? Cod { get; set; }
        public string? Region { get; set; }
        public string? Sucursal { get; set; }
        public string? Articulo { get; set; }
        public string? Seccion { get; set; }
        public string? Invayer { get; set; }
        public double? Traspasoayer { get; set; }
        public double? Consumoayer { get; set; }
        public string? Invhoy { get; set; }
        public DateTime? Captura { get; set; }
        public double? Invformula { get; set; }
        public double? Diferencia { get; set; }
        public double? Mermasayer { get; set; }
        public DateTime? Fechaconsulta { get; set; }
    }
}
