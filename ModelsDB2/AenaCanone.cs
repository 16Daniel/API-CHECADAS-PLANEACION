﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class AenaCanone
    {
        public AenaCanone()
        {
            AenaSubfamilia = new HashSet<AenaSubfamilia>();
        }

        public int IdCanon { get; set; }
        public string? Concepto { get; set; }
        public int? Subconcepto { get; set; }
        public double? PorcentajeVentas { get; set; }
        public double? ImporteUnidad { get; set; }

        public virtual ICollection<AenaSubfamilia> AenaSubfamilia { get; set; }
    }
}
