using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2Prueba
{
    public partial class Embalaje
    {
        public string Codembalaje { get; set; } = null!;
        public string? Descripcion { get; set; }
        public double? Longitud { get; set; }
        public double? Altura { get; set; }
        public double? Anchura { get; set; }
    }
}
