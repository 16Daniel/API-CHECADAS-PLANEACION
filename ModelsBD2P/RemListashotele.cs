﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class RemListashotele
    {
        public int Idhotel { get; set; }
        public int Tipo { get; set; }
        public int Codigo { get; set; }
        public byte[] Version { get; set; } = null!;
        public string? Codigostr { get; set; }

        public virtual Hotele IdhotelNavigation { get; set; } = null!;
    }
}
