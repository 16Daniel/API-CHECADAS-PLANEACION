﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Albcompratalla
    {
        public string Numserie { get; set; } = null!;
        public int Numalbaran { get; set; }
        public string N { get; set; } = null!;
        public int Numlinea { get; set; }
        public string Talla { get; set; } = null!;
        public double? Uds { get; set; }
        public double? Importegastostalla { get; set; }
    }
}
