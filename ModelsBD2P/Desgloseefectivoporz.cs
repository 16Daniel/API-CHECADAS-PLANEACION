﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Desgloseefectivoporz
    {
        public int Fo { get; set; }
        public string Caja { get; set; } = null!;
        public int Z { get; set; }
        public string N { get; set; } = null!;
        public int Cuenta { get; set; }
        public double? Importe { get; set; }
    }
}
