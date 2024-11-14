using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class KpiDetallekpi
    {
        public int Idkpi { get; set; }
        public int Iddetalle { get; set; }
        public int Posicion { get; set; }

        public virtual Kpi IdkpiNavigation { get; set; } = null!;
    }
}
