﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Ftp
    {
        public int Idftp { get; set; }
        public string? Servidor { get; set; }
        public string? Usuario { get; set; }
        public string? Password { get; set; }
        public int? Puerto { get; set; }
        public string? Carpetaimport { get; set; }
        public string? Carpetaexport { get; set; }
    }
}
