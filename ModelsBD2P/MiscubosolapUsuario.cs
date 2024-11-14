using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class MiscubosolapUsuario
    {
        public int Idusuario { get; set; }
        public int Idcubo { get; set; }

        public virtual Miscubosolap IdcuboNavigation { get; set; } = null!;
    }
}
