﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class CmrcParametro
    {
        public int Id { get; set; }
        public int? Tipo { get; set; }
        public int? Grupo { get; set; }
        public int? ValorInt { get; set; }
        public string? ValorString { get; set; }
        public DateTime? ValorFecha { get; set; }
        public decimal? ValorDecimal { get; set; }
        public double? ValorFloat { get; set; }
        public bool? ValorBool { get; set; }
    }
}
