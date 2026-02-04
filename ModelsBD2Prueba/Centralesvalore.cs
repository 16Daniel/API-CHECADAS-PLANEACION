using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2Prueba
{
    public partial class Centralesvalore
    {
        public int Idcentral { get; set; }
        public int Idpermiso { get; set; }
        public int Orden { get; set; }
        public string? Valor { get; set; }

        public virtual Centralespermiso Id { get; set; } = null!;
    }
}
