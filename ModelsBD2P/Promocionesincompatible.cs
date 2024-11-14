using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Promocionesincompatible
    {
        public int Idpromocion { get; set; }
        public int Posicion { get; set; }
        public int? Idpromocionincompatible { get; set; }

        public virtual Promocione IdpromocionNavigation { get; set; } = null!;
    }
}
