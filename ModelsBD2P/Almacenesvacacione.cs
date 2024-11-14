using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Almacenesvacacione
    {
        public string Codalmacen { get; set; } = null!;
        public DateTime Fecha { get; set; }
        public string? Motivo { get; set; }
        public int Codtipodia { get; set; }
    }
}
