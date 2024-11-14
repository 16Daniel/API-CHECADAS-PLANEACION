using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Motivosdescuadre
    {
        public int Idmotivo { get; set; }
        public string? Descripcion { get; set; }
        public byte[] Version { get; set; } = null!;
    }
}
