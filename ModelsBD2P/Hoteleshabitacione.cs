﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Hoteleshabitacione
    {
        public int Idhotel { get; set; }
        public int Codarticulo { get; set; }
        public int Posicion { get; set; }
        public byte[] Version { get; set; } = null!;

        public virtual Articuloshabitacione CodarticuloNavigation { get; set; } = null!;
        public virtual Hotele IdhotelNavigation { get; set; } = null!;
    }
}
