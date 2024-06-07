using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Favoritostiposterminal
    {
        public int Idtipoterminal { get; set; }
        public int Posicion { get; set; }
        public int? Codfavorito { get; set; }

        public virtual Tiposterminal IdtipoterminalNavigation { get; set; } = null!;
    }
}
