﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Albcompracamposlibre
    {
        public string Numserie { get; set; } = null!;
        public int Numalbaran { get; set; }
        public string N { get; set; } = null!;
        public string? Sucursal { get; set; }

        public virtual Albcompracab NNavigation { get; set; } = null!;
    }
}
