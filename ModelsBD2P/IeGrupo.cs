﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class IeGrupo
    {
        public IeGrupo()
        {
            IeInformes = new HashSet<IeInforme>();
        }

        public int IdGrupo { get; set; }
        public string Titulo { get; set; } = null!;

        public virtual ICollection<IeInforme> IeInformes { get; set; }
    }
}
