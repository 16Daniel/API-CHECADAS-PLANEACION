﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDBP
{
    public partial class PedSucArticulo
    {
        public int Id { get; set; }
        public int Codproveedor { get; set; }
        public int Codart { get; set; }
        public bool? Fiscal { get; set; }
        public int? Idperfil { get; set; }
    }
}
