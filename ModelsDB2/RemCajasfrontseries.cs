﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class RemCajasfrontseries
    {
        public int Idfront { get; set; }
        public int Cajafront { get; set; }
        public int Tipodoc { get; set; }
        public string? Serie { get; set; }
        public int? Iddissenycamposlibres { get; set; }
    }
}
