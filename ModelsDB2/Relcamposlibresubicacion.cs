﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Relcamposlibresubicacion
    {
        public int Tabla { get; set; }
        public int Subtipo { get; set; }
        public int Idgrupo { get; set; }
        public int Iddiseny { get; set; }

        public virtual Impresiondoc Id { get; set; } = null!;
    }
}
