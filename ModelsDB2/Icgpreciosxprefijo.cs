using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Icgpreciosxprefijo
    {
        public string Prefijo { get; set; } = null!;
        public double? Precio { get; set; }
        public double? CargoInicial { get; set; }
    }
}
