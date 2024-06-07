﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class RemDispositivoslin
    {
        public int Idfront { get; set; }
        public int Idterminal { get; set; }
        public string Tipodispositivo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public int Posicion { get; set; }
        public string? Secuencia { get; set; }

        public virtual RemDispositivo RemDispositivo { get; set; } = null!;
    }
}
