﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Asuntoscontador
    {
        public string Serieasunto { get; set; } = null!;
        public int? Numeroasunto { get; set; }
        public int? Idservicio { get; set; }
    }
}
