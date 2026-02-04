using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2Prueba
{
    public partial class PmTerminale
    {
        public string Idterminal { get; set; } = null!;
        public string? Nombre { get; set; }
        public string? Tipodocimpreso { get; set; }
        public string? Docimpreso { get; set; }
        public string? Idultimaop { get; set; }
    }
}
