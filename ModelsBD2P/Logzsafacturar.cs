﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Logzsafacturar
    {
        public int Idfront { get; set; }
        public string Tipo { get; set; } = null!;
        public DateTime Fechahoraini { get; set; }
        public int Caja { get; set; }
        public int Z { get; set; }
        public string? Facturada { get; set; }

        public virtual Comunicacionlog Comunicacionlog { get; set; } = null!;
    }
}
