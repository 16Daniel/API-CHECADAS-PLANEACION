using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class RemGrupossecsimpresorarest
    {
        public int Idfront { get; set; }
        public int Idgruposecuencias { get; set; }
        public string? Nombregruposecuencias { get; set; }

        public virtual RemFront IdfrontNavigation { get; set; } = null!;
    }
}
