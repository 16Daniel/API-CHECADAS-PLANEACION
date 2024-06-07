﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class HioposEstadSeries
    {
        public int Id { get; set; }
        public int Serie { get; set; }
        public string? Campolibre { get; set; }

        public virtual HioposEstad IdNavigation { get; set; } = null!;
    }
}
