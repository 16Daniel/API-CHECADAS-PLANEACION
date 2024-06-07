using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Promocionestarifa
    {
        public int Idpromocion { get; set; }
        public int Idtarifav { get; set; }

        public virtual Promocione IdpromocionNavigation { get; set; } = null!;
    }
}
