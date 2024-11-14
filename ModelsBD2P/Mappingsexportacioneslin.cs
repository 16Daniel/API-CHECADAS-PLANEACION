using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Mappingsexportacioneslin
    {
        public int Idexportacion { get; set; }
        public string Clave { get; set; } = null!;

        public virtual Mappingsexportacionescab IdexportacionNavigation { get; set; } = null!;
    }
}
