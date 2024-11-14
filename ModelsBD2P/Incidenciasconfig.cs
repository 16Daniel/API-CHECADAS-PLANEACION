using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Incidenciasconfig
    {
        public int Tipo { get; set; }
        public int Estado { get; set; }
        public int? Tipodocumento { get; set; }
        public string? Codalmacen { get; set; }
    }
}
