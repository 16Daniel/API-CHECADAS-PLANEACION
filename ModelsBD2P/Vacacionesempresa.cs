﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Vacacionesempresa
    {
        public int Codempresa { get; set; }
        public int Codvaclin { get; set; }
        public DateTime Desde { get; set; }
        public DateTime Hasta { get; set; }
        public string? Motivo { get; set; }
    }
}
