using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Ordenesmenudetalle
    {
        public int Codmodificador { get; set; }
        public int Orden { get; set; }
        public double? Dto { get; set; }

        public virtual Modificadorescab CodmodificadorNavigation { get; set; } = null!;
    }
}
