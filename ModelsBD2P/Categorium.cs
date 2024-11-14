using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Categorium
    {
        public int Codcategoria { get; set; }
        public string Descripcion { get; set; } = null!;
        public byte[]? Version { get; set; }
    }
}
