using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Falta
    {
        public string Digitocontrol { get; set; } = null!;
        public int Tipo { get; set; }
        public string Numero { get; set; } = null!;
    }
}
