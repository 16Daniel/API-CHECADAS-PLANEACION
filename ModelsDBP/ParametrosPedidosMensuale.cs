using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsDBP
{
    public partial class ParametrosPedidosMensuale
    {
        public int Id { get; set; }
        public int TiempoDeEntrega { get; set; }
        public int PeriodoDeRevision { get; set; }
        public double NivelDeServicio { get; set; }
        public int MesesConDatos { get; set; }
        public string? DataDivisionPedidos { get; set; }
    }
}
