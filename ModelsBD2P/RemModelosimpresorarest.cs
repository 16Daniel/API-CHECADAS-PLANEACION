using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class RemModelosimpresorarest
    {
        public int Idfront { get; set; }
        public string Modeloimpresora { get; set; } = null!;
        public short? Gruposecuencias { get; set; }

        public virtual RemFront IdfrontNavigation { get; set; } = null!;
    }
}
