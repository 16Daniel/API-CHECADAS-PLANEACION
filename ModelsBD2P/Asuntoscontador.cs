using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Asuntoscontador
    {
        public string Serieasunto { get; set; } = null!;
        public int? Numeroasunto { get; set; }
        public int? Idservicio { get; set; }
    }
}
