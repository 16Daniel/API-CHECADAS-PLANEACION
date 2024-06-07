﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Icgconsultassql
    {
        public int Grupo { get; set; }
        public int Icgconsulta { get; set; }
        public string Tipo { get; set; } = null!;
        public string Tipoparam { get; set; } = null!;
        public string Nombreparam { get; set; } = null!;
        public int Ncampo { get; set; }
        public string Iconsulta { get; set; } = null!;
        public string? Valor { get; set; }
        public int? Codtitulo { get; set; }

        public virtual Icgnombresinforme Icgnombresinforme { get; set; } = null!;
    }
}
