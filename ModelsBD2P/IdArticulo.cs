﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class IdArticulo
    {
        public int Codarticulo { get; set; }
        public Guid Guidarticulo { get; set; }
        public int? Newcodarticulo { get; set; }
        public Guid? Newguidarticulo { get; set; }
        public int? Quienunifica { get; set; }
        public DateTime? Fechaunifica { get; set; }
    }
}
