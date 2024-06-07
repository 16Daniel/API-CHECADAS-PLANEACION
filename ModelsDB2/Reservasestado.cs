﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Reservasestado
    {
        public string Serie { get; set; } = null!;
        public int Idreserva { get; set; }
        public int Idlinea { get; set; }
        public int Tipo { get; set; }
        public string Codestado { get; set; } = null!;
        public int Dias { get; set; }

        public virtual Reserva Reserva { get; set; } = null!;
    }
}
