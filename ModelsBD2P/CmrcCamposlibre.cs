using System;
using System.Collections.Generic;

namespace API_PEDIDOS.ModelsBD2P
{
    public partial class CmrcCamposlibre
    {
        public int Tipo { get; set; }
        public string Nombrecampo { get; set; } = null!;
        public string? Descripcion { get; set; }
    }
}
