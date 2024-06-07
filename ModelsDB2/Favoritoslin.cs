using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Favoritoslin
    {
        public int Codfavorito { get; set; }
        public int Posicion { get; set; }
        public int? Codarticulo { get; set; }
        public int? Tipo { get; set; }

        public virtual Favoritoscab CodfavoritoNavigation { get; set; } = null!;
    }
}
