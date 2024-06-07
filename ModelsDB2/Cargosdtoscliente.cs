using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Cargosdtoscliente
    {
        public int Codcliente { get; set; }
        public int Codigo { get; set; }
        public double? Valor { get; set; }

        public virtual Cliente CodclienteNavigation { get; set; } = null!;
    }
}
