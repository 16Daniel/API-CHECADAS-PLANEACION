﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Comisionesvendedor
    {
        public int Codvendedor { get; set; }
        public DateTime Desde { get; set; }
        public DateTime Hasta { get; set; }
        public int Codcomision { get; set; }

        public virtual Comisionescab CodcomisionNavigation { get; set; } = null!;
        public virtual Vendedore CodvendedorNavigation { get; set; } = null!;
    }
}
