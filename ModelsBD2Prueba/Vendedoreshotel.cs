using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2Prueba
{
    public partial class Vendedoreshotel
    {
        public int Codvendedor { get; set; }
        public int Idhotel { get; set; }

        public virtual Vendedore CodvendedorNavigation { get; set; } = null!;
    }
}
