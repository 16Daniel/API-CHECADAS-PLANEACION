using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDBP
{
    public partial class Calendario
    {
        public int Id { get; set; }
        public int Codsucursal { get; set; }
        public int Codproveedor { get; set; }
        public string Jdata { get; set; } = null!;
    }
}
