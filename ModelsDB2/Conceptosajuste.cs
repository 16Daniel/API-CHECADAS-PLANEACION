using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Conceptosajuste
    {
        public int Id { get; set; }
        public string? Descripcion { get; set; }
        public string? Cuentagastos { get; set; }
        public byte[]? Version { get; set; }
    }
}
