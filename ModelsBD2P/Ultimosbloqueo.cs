﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Ultimosbloqueo
    {
        public int Idfront { get; set; }
        public string Terminal { get; set; } = null!;
        public int? Idbloqueo { get; set; }
    }
}
