using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Urgenciasreparacion
    {
        public int Codurgenciasreparacion { get; set; }
        public string? Urgenciareparacion { get; set; }
        public int? Dias { get; set; }
        public byte[] Version { get; set; } = null!;
    }
}
