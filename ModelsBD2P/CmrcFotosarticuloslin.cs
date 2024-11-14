﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class CmrcFotosarticuloslin
    {
        public int Codarticulo { get; set; }
        public string Talla { get; set; } = null!;
        public string Color { get; set; } = null!;
        public int Codfoto { get; set; }
        public byte[] Version { get; set; } = null!;

        public virtual Articuloslin Articuloslin { get; set; } = null!;
    }
}
