﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Ventastef
    {
        public int Idtipodoc { get; set; }
        public string Serie { get; set; } = null!;
        public int Numero { get; set; }
        public string N { get; set; } = null!;
        public string Terminal { get; set; } = null!;
    }
}
