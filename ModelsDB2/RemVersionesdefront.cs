using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class RemVersionesdefront
    {
        public int Idfront { get; set; }
        public int Idtabla { get; set; }
        public long? Versionimp { get; set; }
        public long? Versionexp { get; set; }
    }
}
