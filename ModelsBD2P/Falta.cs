using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Falta
    {
        public string Digitocontrol { get; set; } = null!;
        public int Tipo { get; set; }
        public string Numero { get; set; } = null!;
    }
}
