﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Presupuestostot
    {
        public string Serie { get; set; } = null!;
        public int Numero { get; set; }
        public string N { get; set; } = null!;
        public int Version { get; set; }
        public int Numlinea { get; set; }
        public double? Bruto { get; set; }
        public double? Dtocomerc { get; set; }
        public double? Totdtocomerc { get; set; }
        public double? Dtopp { get; set; }
        public double? Totdtopp { get; set; }
        public double? Baseimponible { get; set; }
        public double? Iva { get; set; }
        public double? Totiva { get; set; }
        public double? Req { get; set; }
        public double? Totreq { get; set; }
        public double? Total { get; set; }
        public int? Coddto { get; set; }

        public virtual Presupuestoscab Presupuestoscab { get; set; } = null!;
    }
}
