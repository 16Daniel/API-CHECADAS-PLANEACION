﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Galeriaidioma
    {
        public int Idgaleria { get; set; }
        public string Ididioma { get; set; } = null!;
        public string? Descripcion { get; set; }

        public virtual Galerium IdgaleriaNavigation { get; set; } = null!;
    }
}
