﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Comentariosidioma
    {
        public int Codcomentario { get; set; }
        public int Codidioma { get; set; }
        public string? Descripcion { get; set; }

        public virtual Comentario CodcomentarioNavigation { get; set; } = null!;
        public virtual Idioma CodidiomaNavigation { get; set; } = null!;
    }
}
