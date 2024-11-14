using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Imagenesusuario
    {
        public int Codusuario { get; set; }
        public int Tipo { get; set; }
        public byte[]? Imagen { get; set; }
    }
}
