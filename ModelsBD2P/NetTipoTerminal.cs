﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class NetTipoTerminal
    {
        public NetTipoTerminal()
        {
            NetConfigTipoTerminals = new HashSet<NetConfigTipoTerminal>();
            NetTienda = new HashSet<NetTiendum>();
        }

        public int IdTipoTerminal { get; set; }
        public string? Descripcion { get; set; }

        public virtual ICollection<NetConfigTipoTerminal> NetConfigTipoTerminals { get; set; }
        public virtual ICollection<NetTiendum> NetTienda { get; set; }
    }
}
