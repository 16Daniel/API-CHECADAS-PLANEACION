﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Sustituto
    {
        public int Codarticulo { get; set; }
        public int Sustituto1 { get; set; }
        public int? Lastsustituto { get; set; }

        public virtual Articulo1 CodarticuloNavigation { get; set; } = null!;
    }
}
