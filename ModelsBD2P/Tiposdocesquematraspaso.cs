using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Tiposdocesquematraspaso
    {
        public int Tipodoc { get; set; }
        public int Tipoesquema { get; set; }
        public string Configuracion { get; set; } = null!;
        public string Esquema { get; set; } = null!;
    }
}
