using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Balanzasentidade
    {
        public int Id { get; set; }
        public int Tipo { get; set; }
        public string Codigo { get; set; } = null!;

        public virtual Balanza IdNavigation { get; set; } = null!;
    }
}
