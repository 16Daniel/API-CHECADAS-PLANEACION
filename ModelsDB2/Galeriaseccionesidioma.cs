﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Galeriaseccionesidioma
    {
        public int Idgaleria { get; set; }
        public int Numseccion { get; set; }
        public string Codidioma { get; set; } = null!;
        public string? Descripcion { get; set; }

        public virtual Galeriaseccione Galeriaseccione { get; set; } = null!;
    }
}
