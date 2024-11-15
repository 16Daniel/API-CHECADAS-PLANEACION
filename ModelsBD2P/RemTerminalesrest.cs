﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class RemTerminalesrest
    {
        public int Idfront { get; set; }
        public string Terminal { get; set; } = null!;
        public short? Codfo { get; set; }
        public int? Caja { get; set; }
        public int? Idtipoterminal { get; set; }
        public string? Versionexe { get; set; }
        public string? Actualizado { get; set; }
        public bool Conectado { get; set; }

        public virtual RemFront IdfrontNavigation { get; set; } = null!;
    }
}
