using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class IdVenta
    {
        public Guid Guidventa { get; set; }
        public string? Serie { get; set; }
        public int? Numero { get; set; }
        public string? N { get; set; }
    }
}
