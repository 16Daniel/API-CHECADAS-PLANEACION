﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Rutaslin
    {
        public int Codruta { get; set; }
        public int Orden { get; set; }
        public int? Codcliente { get; set; }
        public DateTime? Hora { get; set; }
        public string? Visitado { get; set; }

        public virtual Ruta CodrutaNavigation { get; set; } = null!;
    }
}
