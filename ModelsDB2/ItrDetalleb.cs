using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class ItrDetalleb
    {
        public string Numserie { get; set; } = null!;
        public int Numfactura { get; set; }
        public int Lin { get; set; }
        public DateTime? Fecha { get; set; }
        public string? Descripcion { get; set; }
        public decimal? Uds { get; set; }
        public decimal? Importe { get; set; }
        public decimal? Coste { get; set; }
        public string? Tipo { get; set; }
        public int? Codarticulo { get; set; }
    }
}
