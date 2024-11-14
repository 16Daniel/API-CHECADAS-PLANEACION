using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Hserviciosregiman
    {
        public int Codigo { get; set; }
        public string Descripcion { get; set; } = null!;
        public bool Descatalogado { get; set; }
    }
}
