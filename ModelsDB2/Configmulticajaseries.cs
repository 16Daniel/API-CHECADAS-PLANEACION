﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Configmulticajaseries
    {
        public int Idterminal { get; set; }
        public string Caja { get; set; } = null!;
        public int Tipodoc { get; set; }
        public string Serie { get; set; } = null!;

        public virtual Terminale IdterminalNavigation { get; set; } = null!;
    }
}
