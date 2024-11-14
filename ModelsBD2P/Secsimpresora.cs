using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Secsimpresora
    {
        public string Nombreformato { get; set; } = null!;
        public int Codsecuencia { get; set; }
        public string? Secuencia { get; set; }
    }
}
