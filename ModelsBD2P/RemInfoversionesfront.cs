﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class RemInfoversionesfront
    {
        public int Idfront { get; set; }
        public int Entitat { get; set; }
        public long? Version { get; set; }

        public virtual RemFront IdfrontNavigation { get; set; } = null!;
    }
}