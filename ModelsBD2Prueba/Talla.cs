using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2Prueba
{
    public partial class Talla
    {
        public string Codtalla { get; set; } = null!;
        public int Posicion { get; set; }
        public string Talla1 { get; set; } = null!;
        public byte[]? Version { get; set; }
    }
}
