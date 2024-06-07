﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Facturasventafirma
    {
        public string Serie { get; set; } = null!;
        public int Numero { get; set; }
        public string N { get; set; } = null!;
        public string? Versionfirma { get; set; }
        public string? Firma { get; set; }
        public string? Claveprivada { get; set; }
        public string? Claveacceso { get; set; }
        public int? Estado1 { get; set; }
        public int? Estado2 { get; set; }
        public string? Error { get; set; }

        public virtual Facturasventum Facturasventum { get; set; } = null!;
    }
}
