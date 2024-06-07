using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class RemVersioneslocal
    {
        public int Idtabla { get; set; }
        public string Clave { get; set; } = null!;
        public long? Versionimp { get; set; }
        public long? Versionexp { get; set; }
    }
}
