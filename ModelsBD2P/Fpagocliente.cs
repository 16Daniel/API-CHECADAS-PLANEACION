﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Fpagocliente
    {
        public int Codcliente { get; set; }
        public string Tipo { get; set; } = null!;
        public string Codformapago { get; set; } = null!;
        public int Coddtopp { get; set; }
        public double? Dtopp { get; set; }
        public double? Cantidad { get; set; }
        public string? Serie { get; set; }

        public virtual Cliente CodclienteNavigation { get; set; } = null!;
        public virtual Formaspago CodformapagoNavigation { get; set; } = null!;
    }
}
