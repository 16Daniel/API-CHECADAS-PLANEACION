using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class MotivosabonoIdioma
    {
        public int Idmotivoabono { get; set; }
        public int Codidioma { get; set; }
        public int? Posicion { get; set; }
        public string? Descripcion { get; set; }

        public virtual Motivosabono IdmotivoabonoNavigation { get; set; } = null!;
    }
}
