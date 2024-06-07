using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDBP
{
    public partial class ArticulosProveedor
    {
        public int Id { get; set; }
        public int? Codprov { get; set; }
        public int? Codsucursal { get; set; }
        public int? Codarticulo { get; set; }
    }
}
