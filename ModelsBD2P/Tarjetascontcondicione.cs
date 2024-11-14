using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Tarjetascontcondicione
    {
        public int Idtarjeta { get; set; }
        public int Idfront { get; set; }
        public int? Consrealizadas { get; set; }
        public DateTime? Fecharecalc { get; set; }

        public virtual Tarjeta IdtarjetaNavigation { get; set; } = null!;
    }
}
