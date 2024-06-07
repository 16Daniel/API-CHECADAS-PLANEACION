using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Secsimpresora
    {
        public string Nombreformato { get; set; } = null!;
        public int Codsecuencia { get; set; }
        public string? Secuencia { get; set; }
    }
}
