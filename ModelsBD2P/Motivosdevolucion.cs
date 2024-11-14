﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Motivosdevolucion
    {
        public Motivosdevolucion()
        {
            IdTienda = new HashSet<NetTiendum>();
        }

        public int Idmotivo { get; set; }
        public string? Descripcion { get; set; }
        public string? Codalmacen { get; set; }

        public virtual ICollection<NetTiendum> IdTienda { get; set; }
    }
}
