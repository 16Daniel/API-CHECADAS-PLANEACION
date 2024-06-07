using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class ItExistencia
    {
        public int Codarticulo { get; set; }
        public decimal Existencia { get; set; }
        public decimal Importe { get; set; }
        public DateTime Fecha { get; set; }
    }
}
