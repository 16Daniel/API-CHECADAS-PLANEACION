﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Televentum
    {
        public int Idterminal { get; set; }
        public string Clave { get; set; } = null!;
        public string Subclave { get; set; } = null!;
        public string? Valor { get; set; }
    }
}
