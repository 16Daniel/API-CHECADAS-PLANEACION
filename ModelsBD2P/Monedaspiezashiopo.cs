﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Monedaspiezashiopo
    {
        public long Idpiezamoneda { get; set; }
        public long? Idmoneda { get; set; }
        public decimal? Importe { get; set; }
        public int? Posicion { get; set; }
        public byte[]? Imagen { get; set; }
        public byte[] Version { get; set; } = null!;
        public long? Versioninsert { get; set; }
    }
}
