﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Informefiltro
    {
        public int Idinforme { get; set; }
        public int Numfiltro { get; set; }
        public string? Campo { get; set; }
        public string? Valorinicial { get; set; }
        public string? Valorfinal { get; set; }
        public string? Tipo { get; set; }

        public virtual Informe IdinformeNavigation { get; set; } = null!;
    }
}
