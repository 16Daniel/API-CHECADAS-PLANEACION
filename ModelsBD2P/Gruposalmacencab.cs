﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Gruposalmacencab
    {
        public Gruposalmacencab()
        {
            Gruposalmacenlins = new HashSet<Gruposalmacenlin>();
        }

        public int Idgrupo { get; set; }
        public string? Descripcion { get; set; }
        public int? Codvisible { get; set; }

        public virtual ICollection<Gruposalmacenlin> Gruposalmacenlins { get; set; }
    }
}
