﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class HioposEstadFiltro
    {
        public int Id { get; set; }
        public string Idfiltro { get; set; } = null!;
        public string Valor { get; set; } = null!;

        public virtual HioposEstad IdNavigation { get; set; } = null!;
    }
}
