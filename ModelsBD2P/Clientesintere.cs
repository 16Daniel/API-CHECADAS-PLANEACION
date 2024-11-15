﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Clientesintere
    {
        public int Codcliente { get; set; }
        public int Codinteres { get; set; }
        public DateTime? Fecha { get; set; }
        public int? Codempleado { get; set; }
        public int? Gradointeres { get; set; }
        public string? Observaciones { get; set; }

        public virtual Interese CodinteresNavigation { get; set; } = null!;
    }
}
