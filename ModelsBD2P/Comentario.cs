﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Comentario
    {
        public Comentario()
        {
            Comentariosidiomas = new HashSet<Comentariosidioma>();
        }

        public int Codcomentario { get; set; }
        public string? Descripcion { get; set; }
        public byte[]? Imagen { get; set; }
        public string? Referencia { get; set; }
        public int? Colortexto { get; set; }
        public int? Colorfondo { get; set; }
        public byte[] Version { get; set; } = null!;

        public virtual ICollection<Comentariosidioma> Comentariosidiomas { get; set; }
    }
}
