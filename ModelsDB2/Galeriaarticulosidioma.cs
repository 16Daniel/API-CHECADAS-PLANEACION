﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Galeriaarticulosidioma
    {
        public int Idgaleria { get; set; }
        public int Codarticulo { get; set; }
        public string Codidioma { get; set; } = null!;
        public string? Descripcion { get; set; }

        public virtual Galeriaarticulo Galeriaarticulo { get; set; } = null!;
    }
}
