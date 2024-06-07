﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Albventaconsumo
    {
        public string Numserie { get; set; } = null!;
        public int Numalbaran { get; set; }
        public string N { get; set; } = null!;
        public int Numlinea { get; set; }
        public int Fo { get; set; }
        public string Serie { get; set; } = null!;
        public int Codarticulo { get; set; }
        public double? Consumo { get; set; }
        public string? Codalmacen { get; set; }

        public virtual Albventacab NNavigation { get; set; } = null!;
    }
}
