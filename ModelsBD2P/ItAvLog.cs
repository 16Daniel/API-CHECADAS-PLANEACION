﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class ItAvLog
    {
        public string Numserie { get; set; } = null!;
        public int Numalbaran { get; set; }
        public DateTime Fecha { get; set; }
        public int Reg { get; set; }
    }
}