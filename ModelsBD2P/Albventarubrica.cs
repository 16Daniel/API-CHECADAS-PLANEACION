﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Albventarubrica
    {
        public string Numserie { get; set; } = null!;
        public int Numalbaran { get; set; }
        public string N { get; set; } = null!;
        public byte[]? Rubrica { get; set; }

        public virtual Albventacab NNavigation { get; set; } = null!;
    }
}