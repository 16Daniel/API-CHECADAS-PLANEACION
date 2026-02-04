using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2Prueba
{
    public partial class Vendedoreshorario
    {
        public int Codvendedor { get; set; }
        public string Codalmacen { get; set; } = null!;
        public int Coddia { get; set; }
        public DateTime Inicio { get; set; }
        public DateTime Fin { get; set; }
    }
}
