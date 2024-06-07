﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Facturacionprovlin
    {
        public int Codproveedor { get; set; }
        public int Numconcepto { get; set; }
        public int Numlin { get; set; }
        public int? Tipoimpuesto { get; set; }
        public double? Iva { get; set; }
        public double? Req { get; set; }
        public double? Importe { get; set; }

        public virtual Facturacionprovcab Facturacionprovcab { get; set; } = null!;
    }
}
