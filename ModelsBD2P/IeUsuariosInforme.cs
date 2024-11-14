using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class IeUsuariosInforme
    {
        public int IdUsuario { get; set; }
        public int IdInforme { get; set; }

        public virtual IeInforme IdInformeNavigation { get; set; } = null!;
    }
}
