using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2
{
    public partial class ItTurno
    {
        public int Id { get; set; }
        public int Codarticulo { get; set; }
        public string? L { get; set; }
        public string? M { get; set; }
        public string? Mi { get; set; }
        public string? J { get; set; }
        public string? V { get; set; }
        public string? S { get; set; }
        public string? D { get; set; }
        public string? Activo { get; set; }
    }
}
