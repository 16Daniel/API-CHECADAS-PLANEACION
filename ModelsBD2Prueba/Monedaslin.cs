using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2Prueba
{
    public partial class Monedaslin
    {
        public int Codmoneda { get; set; }
        public double Cantidad { get; set; }
        public byte[]? Imagen { get; set; }

        public virtual Moneda CodmonedaNavigation { get; set; } = null!;
    }
}
