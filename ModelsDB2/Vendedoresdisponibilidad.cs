﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Vendedoresdisponibilidad
    {
        public int Codvendedor { get; set; }
        public int Coddia { get; set; }
        public bool Disponible { get; set; }
        public DateTime Inicio { get; set; }
        public DateTime Fin { get; set; }
    }
}
