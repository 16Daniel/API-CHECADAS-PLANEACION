﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Balanceoscab
    {
        public Balanceoscab()
        {
            Balanceoslins = new HashSet<Balanceoslin>();
        }

        public int Codigo { get; set; }
        public string? Nombre { get; set; }
        public DateTime? Fecha { get; set; }

        public virtual ICollection<Balanceoslin> Balanceoslins { get; set; }
    }
}
