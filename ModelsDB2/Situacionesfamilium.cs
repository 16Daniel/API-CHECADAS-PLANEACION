﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Situacionesfamilium
    {
        public int Codseccion { get; set; }
        public int Codsituacion { get; set; }
        public byte[]? Version { get; set; }

        public virtual Situacione CodsituacionNavigation { get; set; } = null!;
    }
}
