﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Pedcompratalla
    {
        public string Numserie { get; set; } = null!;
        public int Numpedido { get; set; }
        public string N { get; set; } = null!;
        public int Numlinea { get; set; }
        public string Talla { get; set; } = null!;
        public double? Udspedidas { get; set; }
        public double? Udsrecibidas { get; set; }
        public double? Udspendientes { get; set; }
    }
}
