using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class HioposEstadDimensione
    {
        public int Id { get; set; }
        public int Dimension { get; set; }
        public string? Campolibre { get; set; }

        public virtual HioposEstad IdNavigation { get; set; } = null!;
    }
}
