﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class NetConfigTipoTerminal
    {
        public int IdTipoTerminal { get; set; }
        public int IdParametro { get; set; }
        public string? Valor { get; set; }

        public virtual NetTipoTerminal IdTipoTerminalNavigation { get; set; } = null!;
    }
}
