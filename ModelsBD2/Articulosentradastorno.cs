using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2
{
    public partial class Articulosentradastorno
    {
        public int Codarticulo { get; set; }
        public int Idtorno { get; set; }
        public int Idfront { get; set; }
        public byte[]? Version { get; set; }
    }
}
