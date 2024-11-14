using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class ItModCuentasUsuario
    {
        public ItModCuentasUsuario()
        {
            ItPerfiles = new HashSet<ItPerfile>();
        }

        public int Id { get; set; }

        public virtual ICollection<ItPerfile> ItPerfiles { get; set; }
    }
}
