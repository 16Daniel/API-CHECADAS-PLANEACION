using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Imagenesusuario
    {
        public int Codusuario { get; set; }
        public int Tipo { get; set; }
        public byte[]? Imagen { get; set; }
    }
}
