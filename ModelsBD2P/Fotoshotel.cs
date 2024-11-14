using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Fotoshotel
    {
        public int Idhotel { get; set; }
        public Guid Idfoto { get; set; }
        public byte[] Version { get; set; } = null!;
    }
}
