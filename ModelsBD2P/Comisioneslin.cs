﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Comisioneslin
    {
        public int Cod { get; set; }
        public int Grupoarticulo { get; set; }
        public double Desde { get; set; }
        public double Hasta { get; set; }
        public double? Porcentaje { get; set; }

        public virtual Comisionescab CodNavigation { get; set; } = null!;
    }
}
