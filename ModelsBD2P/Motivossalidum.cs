using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Motivossalidum
    {
        public int Idmotivo { get; set; }
        public string? Descripcion { get; set; }
        public byte[]? Version { get; set; }
        public bool Pagado { get; set; }
        public bool? MostrarCpr { get; set; }
        public int? Codtipo { get; set; }
    }
}
