using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2Prueba
{
    public partial class Suplemento
    {
        public int Codigo { get; set; }
        public string? Descripcion { get; set; }
        public string? Porporcentaje { get; set; }
        public int? Incpax { get; set; }
        public int? Codcomentariodef { get; set; }
    }
}
