﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Proveedoresterminal
    {
        public int Idterminal { get; set; }
        public int Id { get; set; }
        public int? Visibilidad { get; set; }

        public virtual Terminale IdterminalNavigation { get; set; } = null!;
    }
}
