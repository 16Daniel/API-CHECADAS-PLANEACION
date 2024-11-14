using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Lugare
    {
        public string Codlugar { get; set; } = null!;
        public string? Descripcion { get; set; }
        public string? Observaciones { get; set; }
        public byte[] Version { get; set; } = null!;
    }
}
