using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2
{
    public partial class Formaspagocajaraiz
    {
        public string Codformapago { get; set; } = null!;
        public string Caja { get; set; } = null!;
        public string? Raizcobro { get; set; }
    }
}
