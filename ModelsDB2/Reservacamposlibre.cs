using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Reservacamposlibre
    {
        public int Idhotel { get; set; }
        public string Serie { get; set; } = null!;
        public int Idreserva { get; set; }

        public virtual Hreservascab Hreservascab { get; set; } = null!;
    }
}
