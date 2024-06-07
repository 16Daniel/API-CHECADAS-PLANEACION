using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class CmrcFoto
    {
        public int Codigo { get; set; }
        public byte[]? Foto { get; set; }
        public byte[] Version { get; set; } = null!;
    }
}
