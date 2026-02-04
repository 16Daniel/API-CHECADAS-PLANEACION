using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2Prueba
{
    public partial class Regimenfacturacion
    {
        public string Tiporegimen { get; set; } = null!;
        public string Codregimen { get; set; } = null!;
        public string? Descripcion { get; set; }
    }
}
