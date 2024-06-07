using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Horarioempleado
    {
        public int Codempleado { get; set; }
        public int Codhorario { get; set; }

        public virtual Horariocab CodhorarioNavigation { get; set; } = null!;
    }
}
