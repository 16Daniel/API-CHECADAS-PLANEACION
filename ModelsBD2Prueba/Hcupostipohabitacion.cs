using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2Prueba
{
    public partial class Hcupostipohabitacion
    {
        public int Idcupo { get; set; }
        public int Tipohabitacion { get; set; }

        public virtual Hcupo IdcupoNavigation { get; set; } = null!;
    }
}
