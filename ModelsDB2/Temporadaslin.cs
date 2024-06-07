﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Temporadaslin
    {
        public int Idtemporada { get; set; }
        public int Idlinea { get; set; }
        public DateTime? Fechainicio { get; set; }
        public DateTime? Fechafin { get; set; }
        public string? Dias { get; set; }

        public virtual Temporadashotel IdtemporadaNavigation { get; set; } = null!;
    }
}
