﻿using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDB2
{
    public partial class Contactosproveedore
    {
        public int Codproveedor { get; set; }
        public string Cargo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string? Telefono { get; set; }
        public string? EMail { get; set; }
        public int? Id { get; set; }
        public bool Facturacion { get; set; }
        public bool Tesoreria { get; set; }
        public string? Mobil { get; set; }

        public virtual Proveedore CodproveedorNavigation { get; set; } = null!;
    }
}
