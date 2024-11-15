﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Favoritosidioma
    {
        public int Codfavorito { get; set; }
        public int Codidioma { get; set; }
        public string? Descripcion { get; set; }
        public byte[]? Version { get; set; }
        public string? Desclarga { get; set; }

        public virtual Favoritoscab CodfavoritoNavigation { get; set; } = null!;
        public virtual Idioma CodidiomaNavigation { get; set; } = null!;
    }
}
