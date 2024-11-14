﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class NetFamiliasTiendum
    {
        public int IdTienda { get; set; }
        public int IdFamilia { get; set; }
        public int? Posicion { get; set; }

        public virtual Favoritoscab IdFamiliaNavigation { get; set; } = null!;
        public virtual NetTiendum IdTiendaNavigation { get; set; } = null!;
    }
}