﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class KpiUsuario
    {
        public int Idkpi { get; set; }
        public int Codusuario { get; set; }
        public int Posicion { get; set; }

        public virtual Kpi IdkpiNavigation { get; set; } = null!;
    }
}
