﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class Serviciosdocumento
    {
        public double Idintervencion { get; set; }
        public string Serie { get; set; } = null!;
        public int Numero { get; set; }
        public int Contador { get; set; }
        public string? Ruta { get; set; }
        public int? Ordenfoto { get; set; }

        public virtual Servicio Servicio { get; set; } = null!;
    }
}
