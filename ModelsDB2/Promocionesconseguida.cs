﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Promocionesconseguida
    {
        public int Idtarjeta { get; set; }
        public int Idfront { get; set; }
        public int Idlinea { get; set; }
        public string? Mostrar { get; set; }

        public virtual Tarjeta IdtarjetaNavigation { get; set; } = null!;
    }
}
