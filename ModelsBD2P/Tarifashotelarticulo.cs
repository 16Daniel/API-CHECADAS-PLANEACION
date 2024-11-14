﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Tarifashotelarticulo
    {
        public int Codtarifa { get; set; }
        public int Codarticulo { get; set; }
        public string Tipo { get; set; } = null!;
        public short? Tipovaloracion { get; set; }
        public byte[] Version { get; set; } = null!;

        public virtual Tarifashotel CodtarifaNavigation { get; set; } = null!;
    }
}
