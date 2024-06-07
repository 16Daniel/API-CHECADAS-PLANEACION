﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class ShowHorariofront
    {
        public int Idfront { get; set; }
        public int? Idhorario { get; set; }
        public byte[]? Version { get; set; }

        public virtual RemFront IdfrontNavigation { get; set; } = null!;
        public virtual ShowHorario? IdhorarioNavigation { get; set; }
    }
}
