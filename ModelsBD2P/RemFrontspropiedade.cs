using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class RemFrontspropiedade
    {
        public int Idfront { get; set; }
        public string Clave { get; set; } = null!;
        public string Subclave { get; set; } = null!;
        public string? Valor { get; set; }

        public virtual RemFront IdfrontNavigation { get; set; } = null!;
    }
}
