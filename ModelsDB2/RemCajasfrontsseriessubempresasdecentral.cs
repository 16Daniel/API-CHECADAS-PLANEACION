﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class RemCajasfrontsseriessubempresasdecentral
    {
        public int Idfront { get; set; }
        public int Cajafront { get; set; }
        public int Subempresa { get; set; }
        public int Tipodoc { get; set; }
        public string? Serie { get; set; }
    }
}
