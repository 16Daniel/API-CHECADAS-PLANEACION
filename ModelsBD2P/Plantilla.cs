using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Plantilla
    {
        public string Tipoplantilla { get; set; } = null!;
        public string Tipocolumna { get; set; } = null!;
        public string? Titulocolumna { get; set; }
        public string? Descripcion { get; set; }
    }
}
