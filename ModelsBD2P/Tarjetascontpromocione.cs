﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Tarjetascontpromocione
    {
        public int Idtarjeta { get; set; }
        public int Idfront { get; set; }
        public int? Puntosacumulados { get; set; }
        public double? Consacumuladas { get; set; }
        public double? Importeacumulado { get; set; }
        public double? Ticketsacumulados { get; set; }

        public virtual Tarjeta IdtarjetaNavigation { get; set; } = null!;
    }
}