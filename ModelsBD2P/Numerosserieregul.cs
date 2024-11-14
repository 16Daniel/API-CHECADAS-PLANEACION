﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Numerosserieregul
    {
        public int Codarticulo { get; set; }
        public string Talla { get; set; } = null!;
        public string Color { get; set; } = null!;
        public string Codalmacen { get; set; } = null!;
        public double? Unidades { get; set; }

        public virtual Articuloslin Articuloslin { get; set; } = null!;
    }
}
