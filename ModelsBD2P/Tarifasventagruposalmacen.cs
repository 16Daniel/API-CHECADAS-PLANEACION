using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Tarifasventagruposalmacen
    {
        public int Idtarifav { get; set; }
        public int Idgrupo { get; set; }

        public virtual Tarifasventum IdtarifavNavigation { get; set; } = null!;
    }
}
