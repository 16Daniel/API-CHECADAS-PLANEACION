using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2
{
    public partial class ItDifFecha
    {
        public int Id { get; set; }
        public string Cod { get; set; } = null!;
        public string Region { get; set; } = null!;
        public string Sucursal { get; set; } = null!;
        public string Articulo { get; set; } = null!;
        public string Seccion { get; set; } = null!;
        public string Invayer { get; set; } = null!;
        public double? Traspasoayer { get; set; }
        public double? Consumoayer { get; set; }
        public string Invhoy { get; set; } = null!;
        public DateTime? Captura { get; set; }
        public double? Invformula { get; set; }
        public double? Diferencia { get; set; }
        public double? Mermasayer { get; set; }
        public DateTime? Fecha { get; set; }
    }
}
