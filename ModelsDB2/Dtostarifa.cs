using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Dtostarifa
    {
        public int Tipo { get; set; }
        public int Orden { get; set; }
        public int? Idtarifav { get; set; }
        public double? Dto { get; set; }

        public virtual Tarifasventum? IdtarifavNavigation { get; set; }
    }
}
