using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Configturno
    {
        public short Diasemana { get; set; }
        public int Posicion { get; set; }
        public DateTime? Horainicio { get; set; }
        public DateTime? Horafin { get; set; }
        public int? Codturno { get; set; }
    }
}
