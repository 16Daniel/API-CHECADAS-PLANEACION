﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Sucursalespermiso
    {
        public int Idsucursal { get; set; }
        public int Idpermiso { get; set; }
        public string? Seleccionado { get; set; }
        public int? Tipo { get; set; }

        public virtual Sucursale IdsucursalNavigation { get; set; } = null!;
    }
}
