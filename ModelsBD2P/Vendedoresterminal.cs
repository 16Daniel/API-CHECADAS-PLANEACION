using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Vendedoresterminal
    {
        public int Codvendedor { get; set; }
        public string Terminal { get; set; } = null!;

        public virtual Vendedore CodvendedorNavigation { get; set; } = null!;
    }
}
