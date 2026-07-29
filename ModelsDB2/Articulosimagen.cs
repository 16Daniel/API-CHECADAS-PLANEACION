using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Articulosimagen
    {
        public int Codarticulo { get; set; }
        public byte[]? Version { get; set; }

        public virtual Articulo CodarticuloNavigation { get; set; } = null!;
    }
}
