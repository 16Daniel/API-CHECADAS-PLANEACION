﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Elementosgrupo
    {
        public int Idgrupo { get; set; }
        public int Codarticulo { get; set; }
        public string? Descripcion { get; set; }

        public virtual Grupoarticulo1 IdgrupoNavigation { get; set; } = null!;
    }
}
