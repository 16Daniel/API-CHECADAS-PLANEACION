using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class RemImpresorasrest
    {
        public int Idfront { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Modelo { get; set; }
        public short? Gruposecuencias { get; set; }

        public virtual RemFront IdfrontNavigation { get; set; } = null!;
    }
}
