using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Inventarioszona
    {
        public DateTime Fecha { get; set; }
        public string Codalmacen { get; set; } = null!;
        public string Zona { get; set; } = null!;
        public double? Recuento { get; set; }

        public virtual Inventario Inventario { get; set; } = null!;
    }
}
