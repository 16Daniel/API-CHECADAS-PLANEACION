using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Dtostipoartic
    {
        public int Tipoarticulo { get; set; }
        public double Desde { get; set; }
        public double Hasta { get; set; }
        public double? Dto { get; set; }
    }
}
