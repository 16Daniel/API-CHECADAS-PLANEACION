﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class KpiDetalle
    {
        public int Iddetalle { get; set; }
        public string? Titulo { get; set; }
        public string? Tabladetalle { get; set; }
        public string? Campodetalle { get; set; }
        public string? Joinsql { get; set; }
        public string? Dominiodetallesql { get; set; }
    }
}
