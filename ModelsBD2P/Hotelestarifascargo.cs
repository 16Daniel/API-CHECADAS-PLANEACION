﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Hotelestarifascargo
    {
        public int Idhotel { get; set; }
        public int Idtarifav { get; set; }
        public int Posicion { get; set; }
        public byte[]? Version { get; set; }

        public virtual Hotele IdhotelNavigation { get; set; } = null!;
        public virtual Tarifasventum IdtarifavNavigation { get; set; } = null!;
    }
}
