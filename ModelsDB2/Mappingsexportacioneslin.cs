using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Mappingsexportacioneslin
    {
        public int Idexportacion { get; set; }
        public string Clave { get; set; } = null!;

        public virtual Mappingsexportacionescab IdexportacionNavigation { get; set; } = null!;
    }
}
