using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class RemConfigdisenysimpresorarest
    {
        public int Idfront { get; set; }
        public string Terminal { get; set; } = null!;
        public int Tipo { get; set; }
        public string? Impresora { get; set; }

        public virtual RemFront IdfrontNavigation { get; set; } = null!;
    }
}
