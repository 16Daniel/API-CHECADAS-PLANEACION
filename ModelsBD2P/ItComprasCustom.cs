using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class ItComprasCustom
    {
        public string Uuid { get; set; } = null!;
        public int Codarticulo { get; set; }
        public decimal Uds { get; set; }
        public DateTime? Fecha { get; set; }
    }
}
