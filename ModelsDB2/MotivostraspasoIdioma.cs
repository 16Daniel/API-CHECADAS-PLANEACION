﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class MotivostraspasoIdioma
    {
        public int Idmotivo { get; set; }
        public int Codidioma { get; set; }
        public int? Posicion { get; set; }
        public string? Descripcion { get; set; }

        public virtual Motivostraspaso IdmotivoNavigation { get; set; } = null!;
    }
}
