using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2Prueba
{
    public partial class Docword
    {
        public int Tipodoc { get; set; }
        public string Serie { get; set; } = null!;
        public int Numero { get; set; }
        public string N { get; set; } = null!;
        public string? Fichero { get; set; }

        public virtual Tiposdoc TipodocNavigation { get; set; } = null!;
    }
}
