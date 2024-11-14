﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class CmrcEnlacesidioma
    {
        public int Idenlace { get; set; }
        public int Ididioma { get; set; }
        public string? Titulo { get; set; }

        public virtual CmrcEnlace IdenlaceNavigation { get; set; } = null!;
    }
}
