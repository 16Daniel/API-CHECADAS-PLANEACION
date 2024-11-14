﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Rango
    {
        public int Codcomision { get; set; }
        public int Posicion { get; set; }
        public double? Desde { get; set; }
        public double? Hasta { get; set; }

        public virtual Comisionescab CodcomisionNavigation { get; set; } = null!;
    }
}
