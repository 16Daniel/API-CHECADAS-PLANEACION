﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Relacionescuentasefectivo
    {
        public string Caja { get; set; } = null!;
        public int Codigo { get; set; }
        public string? Descripcion { get; set; }
        public string? Cuenta { get; set; }
    }
}
