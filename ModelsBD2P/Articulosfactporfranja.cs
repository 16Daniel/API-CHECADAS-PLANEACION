﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Articulosfactporfranja
    {
        public int Codarticulo { get; set; }
        public int Codformato { get; set; }
        public int Minini { get; set; }
        public int Minfin { get; set; }

        public virtual Articulo1 CodarticuloNavigation { get; set; } = null!;
        public virtual Formato CodformatoNavigation { get; set; } = null!;
    }
}
