using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2
{
    public partial class Articulosentradashorario
    {
        public int Codarticulo { get; set; }
        public byte Diasemana { get; set; }
        public DateTime Horainicio { get; set; }
        public DateTime? Horafin { get; set; }
        public byte[]? Version { get; set; }

        public virtual Articulo CodarticuloNavigation { get; set; } = null!;
    }
}
