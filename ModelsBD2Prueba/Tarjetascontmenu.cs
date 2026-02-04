using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2Prueba
{
    public partial class Tarjetascontmenu
    {
        public int Idtarjeta { get; set; }
        public int Codarticulo { get; set; }
        public double? Cantidad { get; set; }

        public virtual Tarjeta IdtarjetaNavigation { get; set; } = null!;
    }
}
