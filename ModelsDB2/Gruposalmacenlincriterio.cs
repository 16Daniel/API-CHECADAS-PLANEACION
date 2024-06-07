﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Gruposalmacenlincriterio
    {
        public int Idgrupo { get; set; }
        public int Idcriterio { get; set; }
        public string Codalmacen { get; set; } = null!;
        public double? Valor { get; set; }

        public virtual Gruposalmacenlin Gruposalmacenlin { get; set; } = null!;
        public virtual Gruposalmacencriterio Id { get; set; } = null!;
    }
}
