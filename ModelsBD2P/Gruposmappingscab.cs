﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Gruposmappingscab
    {
        public Gruposmappingscab()
        {
            Gruposmappingslins = new HashSet<Gruposmappingslin>();
        }

        public int Idgrupo { get; set; }
        public string? Descripcion { get; set; }

        public virtual ICollection<Gruposmappingslin> Gruposmappingslins { get; set; }
    }
}
