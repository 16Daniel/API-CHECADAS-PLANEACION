using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Nominasenlacecontum
    {
        public int Mes { get; set; }
        public int Anyo { get; set; }
        public int EnlaceEmpresa { get; set; }
        public int EnlaceEjercicio { get; set; }
        public int? EnlaceAsiento { get; set; }
        public string? EnlaceUsuario { get; set; }
    }
}
