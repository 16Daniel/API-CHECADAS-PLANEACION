using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Promocionesidioma
    {
        public int Idpromocion { get; set; }
        public int Codidioma { get; set; }
        public string? Descripcion { get; set; }
        public string? Textoimprimir { get; set; }

        public virtual Promocione IdpromocionNavigation { get; set; } = null!;
    }
}
