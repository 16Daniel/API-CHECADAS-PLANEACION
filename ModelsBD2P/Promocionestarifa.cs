using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Promocionestarifa
    {
        public int Idpromocion { get; set; }
        public int Idtarifav { get; set; }

        public virtual Promocione IdpromocionNavigation { get; set; } = null!;
    }
}
