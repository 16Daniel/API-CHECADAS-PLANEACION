﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Mappingsfilessortedfield
    {
        public int Idmap { get; set; }
        public int Idfile { get; set; }
        public int Numcampo { get; set; }
        public string? Campo { get; set; }

        public virtual Mappingsfile Id { get; set; } = null!;
    }
}
