﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Tipostarjetacondicionesrtl
    {
        public int Idtipotarjeta { get; set; }
        public int Idfront { get; set; }
        public int Dia { get; set; }
        public int? Idtarifav { get; set; }
        public double? Dto { get; set; }

        public virtual Tipostarjetum IdtipotarjetaNavigation { get; set; } = null!;
    }
}
