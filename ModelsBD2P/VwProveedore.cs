using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class VwProveedore
    {
        public int Codproveedor { get; set; }
        public string Rfc { get; set; } = null!;
        public string? Nomproveedor { get; set; }
        public int Codarticulo { get; set; }
        public decimal? Uds { get; set; }
    }
}
