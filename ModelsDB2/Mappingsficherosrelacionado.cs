﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Mappingsficherosrelacionado
    {
        public int Idmap { get; set; }
        public int Idrelacion { get; set; }
        public int Posicion { get; set; }
        public int? Idfilecab { get; set; }
        public string? Fieldnamecab { get; set; }
        public int? Idfilelin { get; set; }
        public string? Fieldnamelin { get; set; }

        public virtual Mappingscab IdmapNavigation { get; set; } = null!;
    }
}
