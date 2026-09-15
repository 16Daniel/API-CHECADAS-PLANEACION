using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDBP
{
    public partial class CatProveedoresCompra
    {
        public int Id { get; set; }
        public int Codproveedor { get; set; }
        public string Modulo { get; set; } = null!;
    }
}
