using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class AenaSubfamilia
    {
        public int IdContrato { get; set; }
        public int IdSubfamilia { get; set; }
        public int IdCanon { get; set; }
        public string? Descripcion { get; set; }

        public virtual AenaCanone IdCanonNavigation { get; set; } = null!;
        public virtual AenaContrato IdContratoNavigation { get; set; } = null!;
    }
}
