using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Descuentosclientetipoaviso
    {
        public int Codcliente { get; set; }
        public int Codtipoaviso { get; set; }
        public double? Manodeobra { get; set; }
        public double? Desplazamiento { get; set; }
        public int? Articulos { get; set; }
        public int? Consumibles { get; set; }
        public string? Observaciones { get; set; }
    }
}
