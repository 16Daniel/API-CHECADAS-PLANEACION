using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class HioposEstadMetrica
    {
        public int Id { get; set; }
        public int Metrica { get; set; }

        public virtual HioposEstad IdNavigation { get; set; } = null!;
    }
}
