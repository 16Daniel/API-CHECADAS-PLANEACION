﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Diasfestivo
    {
        public string Tipo { get; set; } = null!;
        public short AO { get; set; }
        public short Mes { get; set; }
        public short Dia { get; set; }
    }
}
