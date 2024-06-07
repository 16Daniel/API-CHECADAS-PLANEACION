using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Motivosdescuadre
    {
        public int Idmotivo { get; set; }
        public string? Descripcion { get; set; }
        public byte[] Version { get; set; } = null!;
    }
}
