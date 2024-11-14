using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class RemAccione
    {
        public int Idfront { get; set; }
        public int Accion { get; set; }
        public int Identidad { get; set; }

        public virtual RemFront IdfrontNavigation { get; set; } = null!;
    }
}
