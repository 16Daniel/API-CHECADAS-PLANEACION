using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Previsione
    {
        public short AO { get; set; }
        public short Mes { get; set; }
        public double? Prevision { get; set; }
        public int? Codmoneda { get; set; }
        public DateTime? Fechaprevision { get; set; }
    }
}
