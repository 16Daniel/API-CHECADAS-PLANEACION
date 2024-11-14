﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Asuntosbloqueo
    {
        public string Serie { get; set; } = null!;
        public int Numero { get; set; }
        public string? Terminal { get; set; }
        public DateTime Fechainibloqueo { get; set; }
        public DateTime Horainibloqueo { get; set; }
        public double Idintervencion { get; set; }

        public virtual Asunto Asunto { get; set; } = null!;
    }
}
