﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Motivostraspaso
    {
        public Motivostraspaso()
        {
            MotivostraspasoIdiomas = new HashSet<MotivostraspasoIdioma>();
        }

        public int Idmotivo { get; set; }
        public string? Descripcion { get; set; }
        public string? Descripcioneditable { get; set; }
        public byte[]? Version { get; set; }

        public virtual ICollection<MotivostraspasoIdioma> MotivostraspasoIdiomas { get; set; }
    }
}
