﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class NetGrupoTiendum
    {
        public NetGrupoTiendum()
        {
            NetTienda = new HashSet<NetTiendum>();
        }

        public int IdGrupoTienda { get; set; }
        public string? Descripcion { get; set; }

        public virtual ICollection<NetTiendum> NetTienda { get; set; }
    }
}
