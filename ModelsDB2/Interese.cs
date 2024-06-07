﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Interese
    {
        public Interese()
        {
            Clientesinteres = new HashSet<Clientesintere>();
        }

        public int Codinteres { get; set; }
        public string? Descripcion { get; set; }

        public virtual ICollection<Clientesintere> Clientesinteres { get; set; }
    }
}
