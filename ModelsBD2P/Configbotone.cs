﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Configbotone
    {
        public int Tipo { get; set; }
        public int Codigo { get; set; }
        public string Clave { get; set; } = null!;
        public int Indice { get; set; }
        public string? Caption { get; set; }
        public int? Valor { get; set; }
        public byte[]? Version { get; set; }
    }
}
