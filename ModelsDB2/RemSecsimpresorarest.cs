﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class RemSecsimpresorarest
    {
        public int Idfront { get; set; }
        public int Idgruposecuencias { get; set; }
        public int Codsecuencia { get; set; }
        public string? Secuencia { get; set; }

        public virtual RemFront IdfrontNavigation { get; set; } = null!;
    }
}
