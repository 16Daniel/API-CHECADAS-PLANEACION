﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Albventamodif
    {
        public string Numserie { get; set; } = null!;
        public int Numalbaran { get; set; }
        public string N { get; set; } = null!;
        public int Numlinea { get; set; }
        public int Fo { get; set; }
        public string Serie { get; set; } = null!;
        public short Nummodif { get; set; }
        public string? Descripcion { get; set; }
        public double? Incprecio { get; set; }
        public int? Codmodif { get; set; }
        public int? Codarticulo { get; set; }
        public short? Orden { get; set; }
        public short? Nivel { get; set; }
        public int? Esarticulo { get; set; }
        public int? Division { get; set; }
        public double? Unidades { get; set; }
        public int? Codformato { get; set; }

        public virtual Albventacab NNavigation { get; set; } = null!;
    }
}
