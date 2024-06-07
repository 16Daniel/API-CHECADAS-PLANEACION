using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Parametro
    {
        public string Clave { get; set; } = null!;
        public string Subclave { get; set; } = null!;
        public string Usuario { get; set; } = null!;
        public string? Valor { get; set; }
        public string? Titulo { get; set; }
    }
}
