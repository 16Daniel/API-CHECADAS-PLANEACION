﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Condicione
    {
        public int Codcondicion { get; set; }
        public string? Conddescripcion { get; set; }
        public double? Manodeobra { get; set; }
        public double? Desplazamiento { get; set; }
        public double? Recambios { get; set; }
        public string? Facturable { get; set; }
        public byte[] Version { get; set; } = null!;
    }
}
