using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Situacionesmacro
    {
        public int Codmacro { get; set; }
        public int Codsituacion { get; set; }

        public virtual Situacione CodsituacionNavigation { get; set; } = null!;
    }
}
