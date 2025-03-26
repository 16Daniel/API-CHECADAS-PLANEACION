using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDBP
{
    public partial class PedSucProveedore
    {
        public int Id { get; set; }
        public int Codproveedor { get; set; }
        public int? Codsucursal { get; set; }
        public int? Idperfil { get; set; }
    }
}
