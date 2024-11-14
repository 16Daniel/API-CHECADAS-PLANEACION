using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Mappingsfilesmainfield
    {
        public int Idmap { get; set; }
        public int Idfile { get; set; }
        public int Numcampo { get; set; }
        public string? Campo { get; set; }

        public virtual Mappingsfile Id { get; set; } = null!;
    }
}
