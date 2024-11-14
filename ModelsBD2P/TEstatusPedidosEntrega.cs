﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class TEstatusPedidosEntrega
    {
        public TEstatusPedidosEntrega()
        {
            TPedidosEntregas = new HashSet<TPedidosEntrega>();
        }

        public int Id { get; set; }
        public string Estatus { get; set; } = null!;
        public string Tipo { get; set; } = null!;
        public bool Active { get; set; }

        public virtual ICollection<TPedidosEntrega> TPedidosEntregas { get; set; }
    }
}
