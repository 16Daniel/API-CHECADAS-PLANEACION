using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Rangohora
    {
        public int Idperiodo { get; set; }
        public string? Descripcion { get; set; }
        public DateTime? Horain { get; set; }
        public DateTime? Horafin { get; set; }
    }
}
