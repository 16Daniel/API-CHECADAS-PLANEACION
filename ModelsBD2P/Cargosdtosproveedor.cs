using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Cargosdtosproveedor
    {
        public int Codproveedor { get; set; }
        public int Codigo { get; set; }
        public double? Valor { get; set; }

        public virtual Proveedore CodproveedorNavigation { get; set; } = null!;
    }
}
