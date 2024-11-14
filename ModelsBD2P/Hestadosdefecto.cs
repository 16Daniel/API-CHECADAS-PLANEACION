using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Hestadosdefecto
    {
        public int Codigo { get; set; }
        public string Idestado { get; set; } = null!;
        public bool? Poner { get; set; }
        public int Idhotel { get; set; }
        public byte[]? Version { get; set; }

        public virtual Hestadoshabitacione IdestadoNavigation { get; set; } = null!;
    }
}
