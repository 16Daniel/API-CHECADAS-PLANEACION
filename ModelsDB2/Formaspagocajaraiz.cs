using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Formaspagocajaraiz
    {
        public string Codformapago { get; set; } = null!;
        public string Caja { get; set; } = null!;
        public string? Raizcobro { get; set; }
    }
}
