using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Acumuladoretencione
    {
        public int Tipo { get; set; }
        public int Mes { get; set; }
        public int Anyo { get; set; }
        public int Codcliprov { get; set; }
        public int Codregimenartic { get; set; }
        public double? Pagado { get; set; }
        public double? Retenido { get; set; }
    }
}
