﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Respuestasasuntoconfigurable
    {
        public string Serie { get; set; } = null!;
        public int Idnumero { get; set; }
        public int Codrespuesta { get; set; }
        public string? Texto { get; set; }
        public double? Numero { get; set; }
        public string? Boolea { get; set; }
        public DateTime? Fecha { get; set; }

        public virtual Asunto Asunto { get; set; } = null!;
    }
}
