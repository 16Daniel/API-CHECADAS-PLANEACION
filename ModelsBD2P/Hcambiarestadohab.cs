﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Hcambiarestadohab
    {
        public int Idlog { get; set; }
        public DateTime? Fecha { get; set; }
        public DateTime? Hora { get; set; }
        public string? Numhab { get; set; }
        public string? Estado { get; set; }
        public string? Usuariocentralita { get; set; }
        public bool? Exportada { get; set; }
        public bool? Descartada { get; set; }
    }
}
