﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Articulosimagene
    {
        public int Codarticulo { get; set; }
        public int Idimagen { get; set; }
        public int Idhotel { get; set; }
        public byte[]? Imagen { get; set; }

        public virtual Articulo1 CodarticuloNavigation { get; set; } = null!;
    }
}
