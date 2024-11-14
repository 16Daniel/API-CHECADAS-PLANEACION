﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Articulosentradashorario
    {
        public int Codarticulo { get; set; }
        public byte Diasemana { get; set; }
        public DateTime Horainicio { get; set; }
        public DateTime? Horafin { get; set; }
        public byte[]? Version { get; set; }

        public virtual Articulo1 CodarticuloNavigation { get; set; } = null!;
    }
}
