﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Vendedoresturnosplan
    {
        public int Codvendedor { get; set; }
        public string Codalmacen { get; set; } = null!;
        public DateTime Fecha { get; set; }
        public int Codturno { get; set; }
    }
}
