﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class TareasautoLog
    {
        public int Idtarea { get; set; }
        public DateTime Fechaejecucion { get; set; }
        public DateTime Horaejecucion { get; set; }
        public string? Observaciones { get; set; }

        public virtual Tareasauto IdtareaNavigation { get; set; } = null!;
    }
}
