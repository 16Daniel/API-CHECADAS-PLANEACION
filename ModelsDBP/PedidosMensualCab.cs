using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDBP
{
    public partial class PedidosMensualCab
    {
        public int Id { get; set; }
        public int Idsucursal { get; set; }
        public int Codproveedor { get; set; }
        public int DivisionPedidos { get; set; }
        public string? Estatus { get; set; }
        public DateTime Fecha { get; set; }
    }
}
