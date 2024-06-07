﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Cajasasignada
    {
        public int Idterminal { get; set; }
        public int Codvendedor { get; set; }
        public string Caja { get; set; } = null!;

        public virtual Terminale IdterminalNavigation { get; set; } = null!;
    }
}
