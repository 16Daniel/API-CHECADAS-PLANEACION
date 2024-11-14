﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Configmulticaja
    {
        public int Idterminal { get; set; }
        public string Caja { get; set; } = null!;
        public int? Numvendedores { get; set; }
        public string? Serieresolucion { get; set; }

        public virtual Terminale IdterminalNavigation { get; set; } = null!;
    }
}
