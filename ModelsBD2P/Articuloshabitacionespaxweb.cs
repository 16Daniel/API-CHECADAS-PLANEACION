﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Articuloshabitacionespaxweb
    {
        public int Codarticulo { get; set; }
        public int Paxadult { get; set; }
        public int Paxnen { get; set; }
        public int Paxbebe { get; set; }

        public virtual Articuloshabitacione CodarticuloNavigation { get; set; } = null!;
    }
}
