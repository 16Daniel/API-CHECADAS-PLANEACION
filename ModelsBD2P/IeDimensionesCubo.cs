﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class IeDimensionesCubo
    {
        public int IdDimension { get; set; }
        public int IdCubo { get; set; }
        public bool? Generar { get; set; }

        public virtual IeCubo IdCuboNavigation { get; set; } = null!;
        public virtual IeDimensione IdDimensionNavigation { get; set; } = null!;
    }
}
