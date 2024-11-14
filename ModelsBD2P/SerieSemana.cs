using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class SerieSemana
    {
        public int Semana { get; set; }
        public int Mes { get; set; }
        public int Año { get; set; }
        public int DiaInicio { get; set; }
        public int DiaFin { get; set; }
    }
}
