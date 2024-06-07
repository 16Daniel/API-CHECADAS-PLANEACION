using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Clientesterminal
    {
        public int Idterminal { get; set; }
        public int Id { get; set; }
        public int? Visibilidad { get; set; }

        public virtual Terminale IdterminalNavigation { get; set; } = null!;
    }
}
