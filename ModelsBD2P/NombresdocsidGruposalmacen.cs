﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class NombresdocsidGruposalmacen
    {
        public short Codgrupo { get; set; }
        public int Coddocumento { get; set; }
        public int Codgrupoalmacen { get; set; }

        public virtual Nombresdocsid Cod { get; set; } = null!;
    }
}
