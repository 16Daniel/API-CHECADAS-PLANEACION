using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Promocionesconseguida
    {
        public int Idtarjeta { get; set; }
        public int Idfront { get; set; }
        public int Idlinea { get; set; }
        public string? Mostrar { get; set; }

        public virtual Tarjeta IdtarjetaNavigation { get; set; } = null!;
    }
}
