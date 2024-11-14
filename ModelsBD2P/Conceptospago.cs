﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Conceptospago
    {
        public int Id { get; set; }
        public string? Descripcion { get; set; }
        public string? Cuentagastos { get; set; }
        public byte[] Version { get; set; } = null!;
        public string? Movcaja { get; set; }
        public bool? Visiblehojacierre { get; set; }
    }
}
