﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Parametro
    {
        public string Clave { get; set; } = null!;
        public string Subclave { get; set; } = null!;
        public string Usuario { get; set; } = null!;
        public string? Valor { get; set; }
        public string? Titulo { get; set; }
    }
}
