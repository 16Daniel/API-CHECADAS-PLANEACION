﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Codigopostal
    {
        public int Idcodpostal { get; set; }
        public string Codpostal { get; set; } = null!;
        public string Codpais { get; set; } = null!;
        public string? Provincia { get; set; }
        public string? Poblacion { get; set; }
        public string? Zona { get; set; }
    }
}
