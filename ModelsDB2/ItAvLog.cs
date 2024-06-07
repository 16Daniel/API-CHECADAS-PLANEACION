using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class ItAvLog
    {
        public string Numserie { get; set; } = null!;
        public int Numalbaran { get; set; }
        public DateTime Fecha { get; set; }
        public int Reg { get; set; }
    }
}
