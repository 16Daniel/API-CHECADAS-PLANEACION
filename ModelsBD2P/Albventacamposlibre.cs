﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Albventacamposlibre
    {
        public string Numserie { get; set; } = null!;
        public int Numalbaran { get; set; }
        public string N { get; set; } = null!;
        public string? Requisicion { get; set; }
        public int? Proveedor { get; set; }

        public virtual Albventacab NNavigation { get; set; } = null!;
    }
}