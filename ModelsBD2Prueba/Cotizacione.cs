using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2Prueba
{
    public partial class Cotizacione
    {
        public DateTime Fecha { get; set; }
        public int Codmoneda { get; set; }
        public double Cotizacion { get; set; }
        public byte[] Version { get; set; } = null!;

        public virtual Moneda CodmonedaNavigation { get; set; } = null!;
    }
}
