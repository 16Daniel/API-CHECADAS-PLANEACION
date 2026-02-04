using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2Prueba
{
    public partial class Articulosimagen
    {
        public int Codarticulo { get; set; }
        public byte[]? Version { get; set; }

        public virtual Articulo1 CodarticuloNavigation { get; set; } = null!;
    }
}
