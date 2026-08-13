using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDBP
{
    public partial class CheckPlaneacionMensual
    {
        public int Id { get; set; }
        public int? Codarticulo { get; set; }
        public int? Codproveedor { get; set; }
    }
}
