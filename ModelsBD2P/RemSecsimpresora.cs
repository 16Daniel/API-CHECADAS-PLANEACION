using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class RemSecsimpresora
    {
        public int Idfront { get; set; }
        public string Nombreformato { get; set; } = null!;
        public int Codsecuencia { get; set; }
        public string? Secuencia { get; set; }

        public virtual RemFront IdfrontNavigation { get; set; } = null!;
    }
}
