﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Hcuposcomentario
    {
        public int Idcupo { get; set; }
        public int Numcomentario { get; set; }
        public int? Codidioma { get; set; }
        public string? Comentario { get; set; }

        public virtual Hcupo IdcupoNavigation { get; set; } = null!;
    }
}
