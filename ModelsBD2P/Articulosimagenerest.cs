﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Articulosimagenerest
    {
        public int Codarticulo { get; set; }
        public byte[]? Imagen { get; set; }
        public byte[]? Version { get; set; }

        public virtual Articulo1 CodarticuloNavigation { get; set; } = null!;
    }
}
