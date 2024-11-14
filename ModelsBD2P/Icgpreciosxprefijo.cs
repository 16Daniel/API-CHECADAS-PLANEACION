using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Icgpreciosxprefijo
    {
        public string Prefijo { get; set; } = null!;
        public double? Precio { get; set; }
        public double? CargoInicial { get; set; }
    }
}
