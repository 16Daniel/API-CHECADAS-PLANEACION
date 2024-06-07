using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Docwordconfig
    {
        public int Tipodoc { get; set; }
        public string? Path { get; set; }
        public string? Plantilla { get; set; }

        public virtual Tiposdoc TipodocNavigation { get; set; } = null!;
    }
}
