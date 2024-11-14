using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Mensaje
    {
        public int Codigo { get; set; }
        public string Emisor { get; set; } = null!;
        public string Receptor { get; set; } = null!;
        public DateTime Fechaini { get; set; }
        public DateTime Fechafin { get; set; }
        public DateTime Fechamodificado { get; set; }
        public string? Mensaje1 { get; set; }
    }
}
