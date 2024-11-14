using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Vacacionesmotivo
    {
        public int Codmotivo { get; set; }
        public string Motivo { get; set; } = null!;
        public bool Pagado { get; set; }
        public byte[]? Version { get; set; }
    }
}
