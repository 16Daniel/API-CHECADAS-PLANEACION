using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2Prueba
{
    public partial class Clientesactividad
    {
        public int Codcliente { get; set; }
        public int Codactividad { get; set; }
        public int? Codcompetencia { get; set; }
        public double? Compras { get; set; }

        public virtual Actividade CodactividadNavigation { get; set; } = null!;
    }
}
