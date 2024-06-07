﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Comunidade
    {
        public string Codpais { get; set; } = null!;
        public int Codigo { get; set; }
        public string? Descripcion { get; set; }

        public virtual Paise CodpaisNavigation { get; set; } = null!;
    }
}
