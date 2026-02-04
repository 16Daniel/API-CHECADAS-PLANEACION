using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2Prueba
{
    public partial class ItRcedi
    {
        public string Serie { get; set; } = null!;
        public int Numpedido { get; set; }
        public string Supedido { get; set; } = null!;
        public string Seriealbaran { get; set; } = null!;
        public int Numeroalbaran { get; set; }
        public string? Numserie { get; set; }
        public DateTime? Fechapedido { get; set; }
    }
}
