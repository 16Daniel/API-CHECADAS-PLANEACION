using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Proveedorescamposlibre
    {
        public int Codproveedor { get; set; }
        public string? Serie { get; set; }
        public string? RecepcionXml { get; set; }
        public string? Planeacion { get; set; }

        public virtual Proveedore CodproveedorNavigation { get; set; } = null!;
    }
}
