using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Parametrostiposterminal
    {
        public string Terminal { get; set; } = null!;
        public string Clave { get; set; } = null!;
        public string? Valor { get; set; }
        public int? Tipo { get; set; }
    }
}
