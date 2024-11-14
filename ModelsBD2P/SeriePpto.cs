﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class SeriePpto
    {
        public string SerieAnp { get; set; } = null!;
        public int Año { get; set; }
        public string TipoCosto { get; set; } = null!;
        public int IdConcepto { get; set; }
        public int? IdDpto { get; set; }
        public string Concepto { get; set; } = null!;
        public decimal? PresupuestoCosto { get; set; }
    }
}
