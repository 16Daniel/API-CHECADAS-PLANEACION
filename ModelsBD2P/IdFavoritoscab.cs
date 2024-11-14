using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class IdFavoritoscab
    {
        public int Codfavorito { get; set; }
        public Guid Guidgrupofavorito { get; set; }

        public virtual Favoritoscab CodfavoritoNavigation { get; set; } = null!;
    }
}
