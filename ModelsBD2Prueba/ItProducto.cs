using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2Prueba
{
    public partial class ItProducto
    {
        public string Rfc { get; set; } = null!;
        public string NoIdentificacion { get; set; } = null!;
        public int Codarticulo { get; set; }
        public string? Umedida { get; set; }
        public decimal? Uds { get; set; }
        public decimal? Puds { get; set; }
        public string? Pumedida { get; set; }
        public decimal? Iuds { get; set; }
        public string? Iumedida { get; set; }
    }
}
