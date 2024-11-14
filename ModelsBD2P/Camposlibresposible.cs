using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Camposlibresposible
    {
        public short Tabla { get; set; }
        public string Campo { get; set; } = null!;
        public short Posicion { get; set; }
        public string? Valor { get; set; }

        public virtual Camposlibresconfig Camposlibresconfig { get; set; } = null!;
    }
}
