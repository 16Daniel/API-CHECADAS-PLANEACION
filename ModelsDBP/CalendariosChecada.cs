﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDBP
{
    public partial class CalendariosChecada
    {
        public int Id { get; set; }
        public int IdPuesto { get; set; }
        public int IdEmpleado { get; set; }
        public string Jdata { get; set; } = null!;
    }
}
