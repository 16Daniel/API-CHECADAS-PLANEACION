﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Serviciosauditorium
    {
        public int Id { get; set; }
        public string Serie { get; set; } = null!;
        public int Numero { get; set; }
        public double Idservicio { get; set; }
        public DateTime Dia { get; set; }
        public DateTime Hora { get; set; }
        public int Codempleado { get; set; }
        public int Estado { get; set; }
        public string? Observaciones { get; set; }
    }
}
