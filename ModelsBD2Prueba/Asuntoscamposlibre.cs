using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2Prueba
{
    public partial class Asuntoscamposlibre
    {
        public string Serie { get; set; } = null!;
        public int Numero { get; set; }

        public virtual Asunto Asunto { get; set; } = null!;
    }
}
