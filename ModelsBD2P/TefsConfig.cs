using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class TefsConfig
    {
        public int Idtef { get; set; }
        public int Idconfig { get; set; }
        public string? Nombre { get; set; }
        public string? Config { get; set; }

        public virtual Tef IdtefNavigation { get; set; } = null!;
    }
}
